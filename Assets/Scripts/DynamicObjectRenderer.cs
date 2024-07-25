using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;
using System.Text;
using UnityEngine.Rendering.Universal;
using Amazon.IoT;
using System.Collections.Specialized;

public class DynamicObjectRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _boxPrefab;
    [SerializeField] private GameObject _tablePrefab;
    [SerializeField] private GameObject _bottlePrefab;
    [SerializeField] private GameObject _laptopPrefab;
    private GameObject _roomPrefab;
    private Transform _robotPlacement;

    private MqttClient client;
    DetectedObjectData detectedObjectData;
    bool isBoxInScene = false;
    bool isTableInScene = false;
    bool isBottleInScene = false;
    bool isLaptopInScene = false;
    string topic;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            string iotEndpoint = "a1cm3c34iajtv7-ats.iot.us-east-1.amazonaws.com";
            int brokerPort = 8883;

            string clientCertLoc = Application.streamingAssetsPath + "/" + "pfx_certificate.pfx";
            string caCertLoc = Application.streamingAssetsPath + "/" + "rootCA.pem";
            topic = "objectDepth";


            UnityWebRequest www = UnityWebRequest.Get(caCertLoc);
            UnityWebRequest www2 = UnityWebRequest.Get(clientCertLoc);
            www.SendWebRequest();
            while (!www.isDone) { }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error loading rootCA");
            }


            www2.SendWebRequest();
            while (!www2.isDone) { }

            if (www2.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error loading certificate.pfx");
            }
            else
            {

                byte[] cacertData = www.downloadHandler.data;
                X509Certificate cacert = new X509Certificate(cacertData);

                byte[] clientCertData = www2.downloadHandler.data;

                var clientCert = new X509Certificate2(clientCertData, "@Gim123");

                client = new MqttClient(iotEndpoint, brokerPort, true,
                                                   cacert,
                                                   clientCert,
                                                   MqttSslProtocols.TLSv1_2);




                client.Connect("unity_object_detection");
                Debug.Log("Object detection is working");
                client.MqttMsgPublishReceived += ClientMqttMsgPublishReceived;

                client.Subscribe(new string[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                if (client.IsConnected)
                {
                    Debug.Log("Object Detection Client Connected!");
                }


            }

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    [System.Serializable]
    private class DetectedObjectData
    {
        public string name;
        public float distance; 
    }

    public class ObjectPlacedResponse
    {
        public string name;
        public bool placed;
    }

    private void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string encodedData = Encoding.UTF8.GetString(e.Message);
        detectedObjectData = JsonUtility.FromJson<DetectedObjectData>(encodedData);
    }

    // Update is called once per frame
    void Update()
    {
        _roomPrefab = GameObject.FindGameObjectWithTag("Room");
        if (_roomPrefab != null)
        {
            _robotPlacement = _roomPrefab.GetComponentsInChildren<Transform>()[1];
            if (_robotPlacement != null)
            {
                if (isBoxInScene == false && detectedObjectData.name == "Box")
                {
                    Debug.Log("Box gonna be placed");
                    PlaceObject(_robotPlacement,_boxPrefab, detectedObjectData.distance, "PlacedBox", new Vector3(0,0.018f,0));
                    Debug.Log("Box placed");
                    isBoxInScene = true;
                    SendJsonMessage(new ObjectPlacedResponse { name = "Box", placed = true });

                } else if (isTableInScene == false && detectedObjectData.name == "Table")
                {
                    PlaceObject(_robotPlacement, _tablePrefab, detectedObjectData.distance, "PlacedTable", new Vector3(0,-0.003f,0));
                    isTableInScene = true;
                    SendJsonMessage(new ObjectPlacedResponse { name = "Table", placed = true });
                } else if (isBottleInScene == false && detectedObjectData.name == "Bottle")
                {
                    PlaceObject(_robotPlacement, _bottlePrefab, detectedObjectData.distance, "PlacedBottle", new Vector3(0, 0.04f, 0));
                    isBottleInScene = true;
                    SendJsonMessage(new ObjectPlacedResponse { name = "Bottle", placed = true });
                } else if (isLaptopInScene == false && detectedObjectData.name == "Laptop")
                {
                    PlaceObject(_robotPlacement, _laptopPrefab, detectedObjectData.distance, "PlacedLaptop", new Vector3(0, 0.04f, 0));
                    isLaptopInScene = true;
                    SendJsonMessage(new ObjectPlacedResponse { name = "Laptop", placed = true });
                }
            }
        }
    }

    private void SendJsonMessage(object messageObject)
    {
        try
        {
            string jsonMessage = JsonUtility.ToJson(messageObject);
            client.Publish(topic, Encoding.UTF8.GetBytes(jsonMessage), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            Debug.Log("object placing JSON message sent to topic: " + topic);
        }
        catch (Exception e)
        {
            Debug.Log("Error sending object placing JSON message: " + e.Message);
        }
    }

    void PlaceObject(Transform robotPlacementTransform,GameObject objectPrefab, float distance, string tagName, Vector3 offset)
    {
        Vector3 objectPosition = robotPlacementTransform.position + robotPlacementTransform.forward * distance * 0.00007f + offset;
        Debug.Log("Robot Placement Transform Position");
        Debug.Log(robotPlacementTransform.position);
        Debug.Log("Object Position");
        Debug.Log(objectPosition);
        GameObject objectInstance = Instantiate(objectPrefab, objectPosition, robotPlacementTransform.rotation);
        objectInstance.tag = tagName;
    }


}
