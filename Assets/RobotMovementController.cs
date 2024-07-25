using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class RobotMovementControllerNew : MonoBehaviour
{
    private UserTouchControllerNew userController;
    private Transform _nextPointerPlacement;
    [SerializeField] private float _speedScale;
    [SerializeField] private float _movementSpeed;
    private GameObject _room;
    private Transform _robotPlacement;

    private MqttClient client;
    private String debugMsg;
    Vector3Data data;
    Vector3 robotPosition;
    Quaternion robotRotation;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            string iotEndpoint = "a1cm3c34iajtv7-ats.iot.us-east-1.amazonaws.com";
            int brokerPort = 8883;

            string clientCertLoc = Application.streamingAssetsPath + "/" + "pfx_certificate.pfx";
            string caCertLoc = Application.streamingAssetsPath + "/" + "rootCA.pem";


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

                var clientCert = new X509Certificate2(clientCertData, "1234");

                client = new MqttClient(iotEndpoint, brokerPort, true,
                                                   cacert,
                                                   clientCert,
                                                   MqttSslProtocols.TLSv1_2);




                client.Connect("pamuditha");
                client.MqttMsgPublishReceived += ClientMqttMsgPublishReceived;
                string topic = "wheelChair/position";
                client.Subscribe(new string[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                if (client.IsConnected)
                {
                    Debug.Log("Client Connected!");
                }


            }

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    [System.Serializable]
    private class Vector3Data
    {
        public string msg;
    }

    // Update is called once per frame
    void Update()
    {
        _room = GameObject.FindGameObjectWithTag("Room");
        if (_room != null)
        {
            _robotPlacement = _room.GetComponentsInChildren<Transform>()[1];
            if (_robotPlacement != null)
            {
                _robotPlacement.localPosition = robotPosition + new Vector3(-0.920000017f, 0.0350000001f, -1.30200005f);
                _robotPlacement.rotation = robotRotation;
            }
        }
    }

    private void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        //Debug.Log("Message received: " + Encoding.UTF8.GetString(e.Message));
        string encodedCoordinates = Encoding.UTF8.GetString(e.Message);
        //Debug.Log(encodedCoordinates);
        data = JsonUtility.FromJson<Vector3Data>(encodedCoordinates);
        string dataSliced = data.msg.Replace("[", "").Replace("]", "");
        string[] dataArr = dataSliced.Split(',');
        robotPosition = new Vector3(float.Parse(dataArr[0]) * 0.0007f, 0, float.Parse(dataArr[1]) * 0.0007f);
        float robotYRotation = -float.Parse(dataArr[2]) + 90.0f;
        robotRotation = Quaternion.Euler(0f, robotYRotation, 0f);
        //Debug.Log(robotPosition);
        Debug.Log("robotRotation");
        Debug.Log(robotRotation);

    }
}
