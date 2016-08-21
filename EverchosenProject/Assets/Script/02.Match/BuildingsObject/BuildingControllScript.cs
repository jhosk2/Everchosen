﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Client;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BuildingControllScript : MonoBehaviour
{

    private GameObject _gameController;
    //생성하는 유닛관련 변수들
    public GameObject UnitPrefab;
    
    public float _unitNumber = 0;
    private GameObject _unit;
    private GameObject _unitNumberPanelPrefab;
    private GameObject _unitNumberPanel;

    public bool PlayerCastle;
    
    public GameObject buildingSettingObject; //오브젝트로 만든것   ui와 오브젝트 2개중 하나사용하면될듯
    private List<Tribe> buildingDataList; //게임오브젝트 변수에서 받아올 빌딩데이터
    //db에서 받아온 값들을 셋팅 해줄 변수들
    public int buildingID;//데이터베이스에서 데이터를 가져올때 필요한 빌딩 아이디
    private float _delayCreateCount;//유닛생성시 스폰 딜레이 카운트 설정
    private float _unitCreateCounter;//유닛 숫자가 1 올라가는데 걸리는 시간

    public float buildingValue;//빌딩 value는 그 빌딩의 유닛이 생산하는 유닛의 체력값 
    public float unitPower;
    public Sprite unitSprite;
    public int playerTeam;//생성할 유닛의 색상설정을 알기위한 변수 

    public int NodeNumber;

    private BuildingLevelData _sendLevelData = new BuildingLevelData();


    void Awake()
    {
        UnitPrefab = Resources.Load<GameObject>("Unit");
       
    }


    // Use this for initialization
    void Start()
    {
        //각 종족에따라 가져올 db설정
        _gameController = GameObject.Find("GameControllerObject");
        if (this.gameObject.tag == _gameController.GetComponent<TeamSettingScript>().playerbuilding)
        {
            buildingDataList = _gameController.GetComponent<TeamSettingScript>().PlayertribeDataToAdd;
        }
        else if (this.gameObject.tag == _gameController.GetComponent<TeamSettingScript>().Enemybuilding)
        {
            buildingDataList = _gameController.GetComponent<TeamSettingScript>().EnemytribeDataToAdd;
        }


        _unitNumberPanelPrefab = Resources.Load<GameObject>("UnitNumberPanel");

        //UNITNUMBER UI 생성

        _unitNumberPanel = Instantiate(_unitNumberPanelPrefab);
        _unitNumberPanel.transform.SetParent(GameObject.Find("UnitNumberUIObject").gameObject.transform);
        _unitNumberPanel.transform.position =
            Camera.main.WorldToScreenPoint(new Vector3(this.gameObject.transform.position.x,
                this.gameObject.transform.position.y, this.gameObject.transform.position.z + 1f));
        _unitNumberPanel.GetComponentInChildren<Text>().text = "" + _unitNumber;


        if (PlayerCastle) //본진이면 아이디 0 , 아니면 1
        {
            buildingID = 0;
        }
        else
        {
            buildingID = 1;
        }
        
        BuildingDataSet(buildingID); //빌딩 데이터 설정
        

       //ui 유닛 넘버 카운트 스타트
        
        // 빌드레벨 설정 오브젝트들
        buildingSettingObject = this.transform.FindChild("BuildingSetting").gameObject;
        buildingSettingObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        UnitCreateCounterFunction();
    }
    
    void BuildingDataSet(int buildingID)
    {
        //건물관련
        this.gameObject.GetComponent<SpriteRenderer>().sprite = buildingDataList[buildingID].BuildingSprite;//선택한 빌딩에 따라 건물스프라이트 가져옴
     
        _delayCreateCount = buildingDataList[buildingID].CreateCount;//생성하는 유닛에 따라 유닛수증가 딜레이 설정
        buildingValue = buildingDataList[buildingID].Value;
        
        //유닛관련
        if (playerTeam == 1)
        {
            unitSprite = buildingDataList[buildingID].BUnitSprite;
        }
        else if (playerTeam == 2)
        {
            unitSprite = buildingDataList[buildingID].RUnitSprite;
        }
        
        //생성할 유닛 sprite
        unitPower = buildingDataList[buildingID].UnitPower;
      
        _unitNumber = 0; // 건물레벨업시 유닛숫자 초기화
        _unitCreateCounter = 0;
        
        UnitNumbersetText();
    }



    public void UnitCreateCounterFunction()//유닛 생성 카운트 함수
    {
        _unitCreateCounter += Time.deltaTime;

        if (_unitCreateCounter > _delayCreateCount)
        {
            _unitNumber++;
            UnitNumbersetText();
            _unitCreateCounter = 0;
        }
    }



    public void UnitSpawn(Vector3 _EndDesPosition) //유닛 생성
    {
     
        if (_unitNumber >= 5)
        {
            for (int i = 0; i < 5; i++) //유닛 생성
            {
                _unit = Instantiate(UnitPrefab);
                _unit.transform.SetParent(this.gameObject.transform);
                _unit.transform.position = this.gameObject.transform.position;
                _unit.transform.rotation = Quaternion.Euler(Vector3.zero);
                _unit.transform.localScale = Vector3.one;
                //생성할 유닛 스폰 아이디 설정
                _unit.GetComponent<NavMeshAgent>().SetDestination(_EndDesPosition); //목적지설정
                _unit.GetComponent<NavMeshAgent>().updateRotation = false;
                // 유닛이 목적지가 아닌곳에서 trigger가 시작되지 않게하기위함
                _unit.GetComponent<UnitControllScript>().destination = _EndDesPosition;
                _unit.GetComponent<BoxCollider>().enabled = false;
                //유닛 공격력 및 sprite설정
                _unit.GetComponent<UnitControllScript>().unitPower = unitPower;
                _unit.GetComponent<UnitControllScript>().UnitSprite = unitSprite;

                //sprite 좌우반전
                if (_unit.transform.position.x > _EndDesPosition.x)
                {
                    _unit.GetComponentInChildren<SpriteRenderer>().flipX = true;
                }
                _unitNumber--;

            }
        }
        else if (_unitNumber > 0 && _unitNumber < 5)
        {
            int createUnitnumber = (int)_unitNumber;
            for (int i = 0; i < createUnitnumber; i++) //유닛 생성
            {
                _unit = Instantiate(UnitPrefab);
                _unit.transform.SetParent(this.gameObject.transform);
                _unit.transform.position = this.gameObject.transform.position;
                _unit.transform.rotation = Quaternion.Euler(Vector3.zero);
                _unit.transform.localScale = Vector3.one;
               
                _unit.GetComponent<NavMeshAgent>().SetDestination(_EndDesPosition);
                _unit.GetComponent<NavMeshAgent>().updateRotation = false;
                // 유닛이 목적지가 아닌곳에서 trigger가 시작되지 않게하기위함
                _unit.GetComponent<UnitControllScript>().destination = _EndDesPosition;
                _unit.GetComponent<BoxCollider>().enabled = false;

                _unit.GetComponent<UnitControllScript>().unitPower = unitPower;
                _unit.GetComponent<UnitControllScript>().UnitSprite = unitSprite;
                _unitNumber--;
                //sprite좌우반전
                if (_unit.transform.position.x > _EndDesPosition.x)
                {
                    _unit.GetComponentInChildren<SpriteRenderer>().flipX = true;
                }
            }
        }
        else
        {
            Debug.Log("유닛이 업성");
        }
        UnitNumbersetText();

    }




    public void UnitNumbersetText() // 유닛number text생성
    {
        _unitNumberPanel.GetComponentInChildren<Text>().text = "" + _unitNumber;
    }


    public void DestroythisBuilding() //침공당해서 숫자가 적어졌을경우 모두파괴
    {

     GameObject buildingPrefab;
     GameObject building;
        switch (this.gameObject.tag)
        {
            case "Player1building":
                buildingPrefab = Resources.Load<GameObject>("Player2building");
                building = Instantiate(buildingPrefab);
                building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                building.transform.position = this.gameObject.transform.position;
                building.transform.localScale = this.gameObject.transform.localScale;
                building.transform.localRotation = Quaternion.Euler(Vector3.zero);

                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber] = new GameObject();
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber] = building;
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber]
                    .GetComponent<BuildingControllScript>().NodeNumber = NodeNumber;


                break;
            case "Player2building":
                buildingPrefab = Resources.Load<GameObject>("Player1building");
                building = Instantiate(buildingPrefab);
                building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                building.transform.position = this.gameObject.transform.position;
                building.transform.localScale = this.gameObject.transform.localScale;
                building.transform.localRotation = Quaternion.Euler(Vector3.zero);

                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber] = new GameObject();
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber] = building;
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber]
                  .GetComponent<BuildingControllScript>().NodeNumber = NodeNumber;

                break;
        }
     
        Destroy(this.gameObject);
        Destroy(_unitNumberPanel);
    }
    

    public void buildingSet1()
    {
        int offsetbuildingID = buildingID;
        buildingID = 2;
        if (buildingID != buildingDataList[offsetbuildingID].BuildingID)
        {
            BuildingDataSet(buildingID);
            LevelSend(buildingID);
           
        }
        else
        {
            Debug.Log("이미 같은 종류의 건물입니다.");
        }
    }

    public void buildingSet2()
    {
        int offsetbuildingID = buildingID;
        buildingID = 3;
        if (buildingID != buildingDataList[offsetbuildingID].BuildingID)
        {
            BuildingDataSet(buildingID);
            LevelSend(buildingID);
        }
        else
        {
            Debug.Log("이미 같은 종류의 건물입니다.");
        }

    }

    public void buildingSet3()
    {
        int offsetbuildingID = buildingID;
        buildingID = 4;
        if (buildingID != buildingDataList[offsetbuildingID].BuildingID)
        {
            BuildingDataSet(buildingID);
            LevelSend(buildingID);
        }
        else
        {
            Debug.Log("이미 같은 종류의 건물입니다.");
        }

    }
    
    public void LevelSend(int lv)
    {
        _sendLevelData.BuildingNode = NodeNumber;
        _sendLevelData.BuildingLevel = lv;
        ClientNetworkManager.Send("Option", _sendLevelData);
    }
    
}



/*
    IEnumerator UnitNumberCounter(float spawnCount) //유닛스폰시간
    {
       
        while (_unitNumberPanel)
        {
            yield return new WaitForSeconds(spawnCount);
            {
                _unitNumber++;
                unitNumbersetText();
            }
        }
        yield break;
    }
*/