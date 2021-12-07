using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Networking;

public class ARPlacemnet : MonoBehaviour
{
    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    private GameObject spawnedObject;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;

    private string base_url = "http://192.168.0.102/ar_api";

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();

    }

    // need to update placement indicator, placement pose and spawn 
    void Update()
    {
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
         {
            ARPlaceObject();
        }


        UpdatePlacementPose();
        UpdatePlacementIndicator();


    }
    void UpdatePlacementIndicator()
    {
       // if (spawnedObject == null && placementPoseIsValid)
        //{
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
       /* }
        else
        {
            placementIndicator.SetActive(false);
        }*/
    }

    void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
        }
    }

    void ARPlaceObject()
    {
        spawnedObject = Instantiate(arObjectToSpawn, PlacementPose.position, PlacementPose.rotation);
        //Debug.Log("Position: " + PlacementPose.position.ToString());
        //Debug.Log("Rotation: " + PlacementPose.rotation);
        StartCoroutine(UploadData(PlacementPose.position.ToString(), PlacementPose.rotation.ToString()));
    }

    IEnumerator UploadData(string position, string rotation)
    {
        WWWForm form = new WWWForm();
        form.AddField("position", position);
        form.AddField("rotation", rotation);

        UnityWebRequest www = UnityWebRequest.Post(base_url + "/home/store", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("MyLog" + www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }


}
