using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ObjectDisplayControl : MonoBehaviour
{
    private UdpClient receiver;
    private int RECEIVE_PORT = 25000;
    private string SEND_HOST = "127.0.0.1";
	/*
    private UdpClient sender;
    private int SEND_PORT = 25001;
	private int command = 0;
	private int testCommand = 0;
	*/
    public GameObject[,] dispObj = new GameObject[2,3];
	
	public AudioClip announceRight;
	public AudioClip announceLeft;
	public AudioClip announceOpen;
	public AudioClip announceClose;
	AudioSource audioSource;
	
	private double elapsedTime = 0;
	private int testCommand = 0;
	private int prevCommand = 0;

	// Use this for initialization
	void Start () {
        receiver = new UdpClient(RECEIVE_PORT);
        receiver.Client.ReceiveTimeout = 1000;
		
		/*
        sender = new UdpClient();
        sender.Connect(SEND_HOST, SEND_PORT);
		*/
		audioSource = GetComponent<AudioSource>();
		
        //object initialize. fetch object
        dispObj[0,0] = GameObject.Find("BallL");
        dispObj[0,1] = GameObject.Find("BallR");
        dispObj[1,0] = GameObject.Find("StickL");
        dispObj[1,1] = GameObject.Find("StickR");
		
        dispObj[0,2] = GameObject.Find("Ball");
        dispObj[1,2] = GameObject.Find("Stick");
		/*
		dispObj[0,0].SetActive(false);
		dispObj[0,1].SetActive(false);
		dispObj[1,0].SetActive(false);
		dispObj[1,1].SetActive(false);
		*/
		//, and display none
		SetDisplayNone();
	}
	
	// Update is called once per frame
    void Update()
    {
		int command = 0;
		elapsedTime += Time.deltaTime; //経過時間
		command = GetUDPCommand();
		//command = CreateTestCommand();
		if (command != prevCommand) {
			SetMultiDisplayPattern(command);
		}
		prevCommand = command;
	}
	int GetUDPCommand() {
        IPEndPoint remoteEP = null;
        byte[] data = receiver.Receive(ref remoteEP);
		int command = 0;
        //Debug.LogFormat("{0},{1},{2},{3},{4},{5},{6},{7}",data[0],data[1],data[2],data[3],data[4],data[5],data[6],data[7]);
        //Debug.Log(BitConverter.ToDouble(data, 0));
		//Array.Reverse(data);
        //Debug.LogFormat("{0},{1},{2},{3},{4},{5},{6},{7}",data[0],data[1],data[2],data[3],data[4],data[5],data[6],data[7]);
        //Debug.Log(BitConverter.ToInt32(data, 0));
        //receiveData = BitConverter.ToInt32(data, 0);
        //Debug.Log(data);
		
		//command = (int)BitConverter.ToDouble(data, 0);
		command = BitConverter.ToInt32(data, 0);
		return command;
	}
	
	int CreateTestCommand() {
		int command = 0;
		int preDisplayTime = 3;
		int displayTime    = 5;
		int nonDisplayTime = 2;
		if(elapsedTime < preDisplayTime) 
		{
			command = testCommand * 10;
		} 
		else if (elapsedTime < (preDisplayTime + displayTime)) 
		{
			command = testCommand;
		} 
		else if (elapsedTime < (preDisplayTime + displayTime + nonDisplayTime)) 
		{
			command = 0;
		} 
		else 
		{ 
			//reset to next pattern
			elapsedTime = 0; //reset pattern period
			testCommand++;    //next pattern
			if(testCommand == 9) testCommand = 1;
		}
		
		return command;
	}
	
	int GetKeyCommand() {
		int command = 0;
		/*
		if(Input.Getkey(KeyCode.Q)) {
		} else if(Input.Getkey(KeyCode.W)) {
		} else if(Input.Getkey(KeyCode.E)) {
		} else if(Input.Getkey(KeyCode.R)) {
		} else if(Input.Getkey(KeyCode.A)) {
		} else if(Input.Getkey(KeyCode.S)) {
		} else if(Input.Getkey(KeyCode.D)) {
		} else if(Input.Getkey(KeyCode.F)) {
		}
		*/
		return command;
	}
	
	/*
	void SetSingleDisplayPattern(int command){
		switch (command)
        {
            case 1:
                ball.SetActive(true);
                stick.SetActive(false);
                break;
            case 2:
                ball.SetActive(false);
                stick.SetActive(true);
                break;
            default:
                ball.SetActive(false);
                stick.SetActive(false);
                break;
        }
	}
	*/
	

	void SetDisplayNone() {
		int i = 0, j = 0;
		for (i = 0;i < 2;i++) {
			for (j = 0;j < 3;j++) {
				dispObj[i,j].SetActive(false);
			}
		}
	}		
	
	void SetMultiDisplayPattern(int command) {
		Debug.Log(command);
		void AnnounceLeftOrRight(char place) {
			if (place == 'L') {
				Debug.Log("左");
				audioSource.PlayOneShot(announceLeft);
			}
			if (place == 'R') {
				Debug.Log("右");
				audioSource.PlayOneShot(announceRight);
			}
		}
		void AnnounceOpenOrClose(char state) {
			if (state == 'O') {
				Debug.Log("開");
				audioSource.PlayOneShot(announceOpen);
			}
			if (state == 'C') {
				Debug.Log("閉");
				audioSource.PlayOneShot	(announceClose);
			}
		}
		
		void SetObjActive(char shape, char place) {
			int n = 0,k = 0;	
			if (shape == 'B') n = 0;
			if (shape == 'S') n = 1;
			
			if (place == 'L') k = 0; //Left
			if (place == 'R') k = 1; //Right
			if (place == 'C') k = 2; //Center
			dispObj[n,k].SetActive(true);
		}
		
		SetDisplayNone();
		switch(command)
		{
			case 10:
			case 30:
			case 50:
			case 70:
				AnnounceLeftOrRight('L');
				break;
			case 20:
			case 40:
			case 60:
			case 80:
				AnnounceLeftOrRight('R');
				break;
			case 1:
			case 2:
				SetObjActive('B','L');SetObjActive('B','R');
				break;
			case 3:
			case 4:
				SetObjActive('B','L');SetObjActive('S','R');
				break;
			case 5:
			case 6:
				SetObjActive('S','L');SetObjActive('B','R');
				break;
			case 7:
			case 8:
				SetObjActive('S','L');SetObjActive('S','R');
				break;
				
			case 300:
				SetDisplayNone();
				AnnounceOpenOrClose('O');
				break;
			case 303:
				SetDisplayNone();
				AnnounceOpenOrClose('C');
				break;
			case 301:
				SetObjActive('B','C');
				break;
			case 302:
				SetObjActive('S','C');
				break;
				
			case 0:
				SetDisplayNone();
				break;
			default:
				SetDisplayNone();
				break;
		}
	}
	
	
}
