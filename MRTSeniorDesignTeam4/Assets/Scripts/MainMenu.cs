﻿using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Input;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class MainMenu : Singleton<MainMenu> {

    private GestureRecognizer gestureRecognizer;
    private SpaceUnderstanding spaceUnderstanding;
    private SpatialMappingManager spatialMappingManager;
    private bool mobileMenu = false;
    private bool billboard = true;
    private int mainMenu_ButtonCount = 0; 

    // Use this for initialization
    public void Start()
    {
        spaceUnderstanding = SpaceUnderstanding.Instance;

        // Start mapping room, hide mesh 
        spatialMappingManager = SpatialMappingManager.Instance;
        spatialMappingManager.DrawVisualMeshes = false;
        spatialMappingManager.StartObserver();

        // Start to recognize gestures 
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        gestureRecognizer.StartCapturingGestures();

    }

    private void menuMove()
    {
        billboard = false;

        RaycastHit hitInfo;

        int raymask = 1 << 5;
        raymask = ~raymask;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 10, raymask))
        {
            this.transform.position = hitInfo.point + GazeManager.Instance.Normal * .05f;
            this.transform.forward = -hitInfo.normal;

        }
        else
        {
            Vector3 Position = Camera.main.transform.position;
            Vector3 Normal = Camera.main.transform.forward;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (mobileMenu)
        {
            menuMove();
        }

        if (billboard)
        {
            this.transform.forward = Camera.main.transform.forward;
        }
    }

    public void Scan_Room_Button_Clicked()
    {

        // Check to make sure Gesture Recognizer is instantiated 
        if (gestureRecognizer != null)
        {
            if(mainMenu_ButtonCount == 0)
            {
                spaceUnderstanding.ShowScan();

                GameObject panelButton = GameObject.FindGameObjectWithTag("Scan_Button_Text"); //.transform.FindChild("Text").GetComponent<GUIText>();

                Text buttonText = panelButton.GetComponent<Text>();

                buttonText.text = "Stop Scan";

                mainMenu_ButtonCount++;

            }
            else
            {
                spatialMappingManager.DrawVisualMeshes = false;
                Place_Menu();
            }
        }
        else
        {
            Debug.Log("Gesture Recognizer must be instantiated");
        }
    }

    public void Place_Menu()
    {
            mobileMenu = true;

            GameObject menuPanel = this.transform.FindChild("AssessmentPanel").gameObject;
            GameObject startCanvas = this.transform.FindChild("FirstMenu").gameObject;

            //
            //  First set the assessmentPanel to be visable
            //  Then set the assessmentPanel's position to be where the damage info's position.
            //  then hide damageInfoPanel.
            //

            menuPanel.SetActive(true);
            menuPanel.transform.position = startCanvas.transform.position;
            startCanvas.SetActive(false);

            gestureRecognizer.TappedEvent += Stop_Menu_Moving;

            mainMenu_ButtonCount++;
    }

    private void Stop_Menu_Moving(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        mobileMenu = false;

        gestureRecognizer.TappedEvent -= Stop_Menu_Moving;

    }
    
}
