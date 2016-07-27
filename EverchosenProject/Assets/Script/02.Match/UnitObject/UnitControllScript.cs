﻿using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class UnitControllScript : MonoBehaviour
{
    private GameObject GameControllerObject;
    public int Team;

    private UnitDataBase unitDB;
    public int UnitId; //건물이 변경됨에 따라 building으로부터 설정되는 아이디값 변경 현재는 0이면 기본보병 1이면 기마병
    private Unit unitToAdd;
    public float unitDamageAmount;
    public float unitSupportAmount;


    public Vector3 destination;
    // Use this for initialization
    void Start ()
    {
       
        GameControllerObject = GameObject.Find("GameControllerObject");
        unitDB = GameControllerObject.GetComponent<UnitDataBase>();//unitDatabase
        Team = GameControllerObject.GetComponent<TeamSettingScript>().playerTeam;//유닛 팀설정

        
        unitDataSet(UnitId); //생성될 유닛 데이터 설정 unitid는 buildingControllerScript에서 unit을 spawn할시 값 초기화
        
        
    }
	
	// Update is called once per frame
	void Update () {
	    if (Vector3.Distance(this.transform.position, destination) < 2)//목적지로 설정한 곳에 도착했을때만 trigger온
	    {
	        this.GetComponent<BoxCollider>().enabled = true;
	    }
	}



    void unitDataSet(int UnitID)
    {
       unitToAdd = unitDB.FetchUnitByID(UnitID);//아이디에 따른 유닛 데이터 정보를 받아옴

        this.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = unitToAdd.Sprite;
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameControllerObject.GetComponent<TeamSettingScript>().Enemybuilding)
        {

            unitCalculateAttack(other);
            other.GetComponent<BuildingControllScript>()._unitNumber -= unitDamageAmount;//(int)unitToAdd.Power; ; //공격력에따라 데미지 변경
            other.GetComponent<BuildingControllScript>().unitNumbersetText();
            if(other.GetComponent<BuildingControllScript>()._unitNumber<=0)
            {
                other.GetComponent<BuildingControllScript>().DestroythisBuilding();
            }
            
            Destroy(this.gameObject);
        }
        else if (other.tag == GameControllerObject.GetComponent<TeamSettingScript>().playerbuilding) //&& other.gameObject!=this.gameObject.transform.parent.gameObject)
        {
            unitCalculateSupport(other);
            Debug.Log(unitSupportAmount);
            other.GetComponent<BuildingControllScript>()._unitNumber += unitSupportAmount;//(int)unitToAdd.Value;// 값에 따라 +변경
            other.GetComponent<BuildingControllScript>().unitNumbersetText();
            Destroy(this.gameObject);
        }
    }


    void unitCalculateAttack(Collider other) //적건물을 공격할때 식
    {
        
            unitDamageAmount =
                (float) Math.Round( unitToAdd.Power/ other.GetComponent<BuildingControllScript>().buildingToAdd.Value, 2); //유닛의 공격력/건물이 가지고 있는유닛의 value값
       
    }

    // 2, 1 일경우 2,1.5일경우 3,1 .. 3,1.5
    // 0.5 데미지   0.75데미지, 0.33데미지.. 0.5데미지

    void unitCalculateSupport(Collider other) // 우리진영건물을 지원할때 식
    {
            unitSupportAmount =
                (float)Math.Round( unitToAdd.Power/ other.GetComponent<BuildingControllScript>().buildingToAdd.Value, 2); 
    }
}