using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    enum Act
    {
        I,
        II,
        III,
        IV
    }

    [SerializeField] Act currentAct = Act.I;

    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float maxVDistance;
    [SerializeField] Rigidbody rb;
    [SerializeField] CinemachineVirtualCamera mainCamera;

    bool isRightRotate = false;
    bool haveKey = false;
    bool haveCoin = false;

    private float currentZPosition;
    [SerializeField] private float upXRotation;
    [SerializeField] private float downXRotation;
    [SerializeField] string playerKeyword;
    [SerializeField] private float customRotation;
    private bool isWalkForward;
    private bool isLookDown = false;
    private bool isLookUp = false;

    [SerializeField] List<string> keywordList = new();

    NavMeshAgent agent;

    [Header("Act 1,2 StopPoints")]
    [SerializeField] Transform codeDoorStopPointI;
    [SerializeField] Transform shelveStopPoint;
    [SerializeField] Transform storageStopPoint;
    [SerializeField] Transform trashStopPoint;
    [SerializeField] Transform testStopPoint;
    [SerializeField] Transform testStopPointII;

    [Header("Door to be Open")]
    [SerializeField] Transform doorI;
    [SerializeField] Transform codeDoor;

    [Header("Act1,2 Transform")]
    [SerializeField] Transform shelve;
    [SerializeField] Transform storage;
    [SerializeField] Transform trash;
    [SerializeField] Transform coin;
    [SerializeField] GameObject drawer;

    [SerializeField] bool move = false;
    private bool isLookCode = false;

    [Header("ActIII Transform")]
    [SerializeField] Transform actIIIDoor;
    [SerializeField] Transform locker;
    [SerializeField] Transform vent;
    [SerializeField] Transform crib;
    [SerializeField] Transform box;
    [SerializeField] Transform VHS;
    [SerializeField] Transform exitDoor;
    [SerializeField] GameObject ventWall;
    [SerializeField] Transform table;
    [SerializeField] Transform wallRight;
    [SerializeField] Transform lastDoor;
    [SerializeField] Transform text;
 
    [Header("ActIII StopPoints")]
    [SerializeField] Transform returnPoint;
    [SerializeField] Transform lockerStopPoint;
    [SerializeField] Transform boxStopPoint;
    [SerializeField] Transform ventStopPoint;
    [SerializeField] Transform cribStopPoint;
    [SerializeField] Transform exitStopPoint;
    [SerializeField] Transform SlideStopPoint;
    [SerializeField] Transform SlidePoint;
    [SerializeField] Transform TableStopPoint;
    [SerializeField] Transform lastDoorStopPoint;

    bool haveScrewdriver = false;

    private event EventHandler OnLookCode;
    private event EventHandler OnFullRotate;
    private event EventHandler OnRotateLeft;
    private event EventHandler OnGameStart;
    private event EventHandler OnActIIStart;
    private event EventHandler OnActIIIStart;
    private event EventHandler OnCrib;
    private event EventHandler OnLocker;
    private event EventHandler OnVent;
    private event EventHandler OnTable;

    bool hasTop = false;
    bool hasBottom = false;
    bool isBegin = false;
    [SerializeField] AddPlayerChat chatSystem;
    private bool isleftRotate;

    private Transform currentPosition;

    [SerializeField] List<Transform> visitedPoint = new();

    [SerializeField] AudioClip voiceOver1;
    [SerializeField] AudioClip voiceOver2;
    [SerializeField] AudioClip babyCry;
    [SerializeField] AudioClip morseCode;

    [SerializeField] AudioSource baby;
    [SerializeField] AudioSource morse;

    [SerializeField] GameObject Zombie;
    [SerializeField] GameObject leg;
    [SerializeField] Canvas staticCanvas;
    private bool isSliding = false;
    private bool hasBack = false;
    [SerializeField]private Transform backPos;

    // Start is called before the first frame update
    void Start()
    {
        currentZPosition = transform.position.z;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        Debug.Log(customRotation);
        agent.stoppingDistance = 0;
        OnLookCode += LookCode;
        OnFullRotate += RotateRight;
        OnRotateLeft += RotateLeft;
        OnGameStart += ActIChat;
        OnActIIStart += ActIIChat;
        OnActIIIStart += ActIIIChat;
        OnCrib += FirstCrib;
        OnLocker += FirstLocker;
        OnVent += FirstVent;
        OnTable += FirstTable;
        agent.stoppingDistance = .1f;
        //agent.SetDestination(TableStopPoint.position);
    }

    // Update is called once per frame
    void Update()
    {

        

        if (move)
        {
            move = false;
        }

        if (isLookDown)
        {
            LookDown();
        }

        if (isLookUp)
        {
            LookUp();
        }

        if (isLookCode)
        {
            OnLookCode?.Invoke(this, EventArgs.Empty);
        }

        if (isRightRotate)
        {
            OnFullRotate?.Invoke(this, EventArgs.Empty);
        }
        if (isleftRotate)
        {
            OnRotateLeft?.Invoke(this, EventArgs.Empty);
        }

        //if (agent.enabled)
        //{
        //    if(agent.remainingDistance <= 0.01 && agent.remainingDistance != 0)
        //        StartCoroutine(ResetAgent());
        //}
            
        switch (currentAct)
        {
            case Act.I:
                if (isBegin == false)
                    OnGameStart?.Invoke(this, EventArgs.Empty);
                if (playerKeyword.Contains("top", StringComparison.CurrentCultureIgnoreCase))
                {
                    isLookUp = true;

                }
                else if (playerKeyword.Contains("bottom", StringComparison.CurrentCultureIgnoreCase))
                {
                    isLookDown = true;
                }
                else if (playerKeyword.Contains("key", StringComparison.CurrentCultureIgnoreCase))
                {
                    TakeKey();
                }

                break;
            case Act.II:
                if (agent.enabled == false)
                    agent.enabled = true;

                if (playerKeyword.Contains("shelve", StringComparison.CurrentCultureIgnoreCase))
                {
                    agent.SetDestination(shelveStopPoint.position);
                    mainCamera.LookAt = shelve;
                    if (agent.remainingDistance <= 0.1 && mainCamera.LookAt == shelve)
                    {
                        currentPosition = shelve;
                        visitedPoint.Add(shelveStopPoint);
                        playerKeyword = "";
                        keywordList.Add("Coin");
                        StartCoroutine(ChatDelay(4f, "LunarWanderer19: There is Copper Coin at the newspaper "));
                        if (visitedPoint.Count == 3 && !drawer.activeInHierarchy)
                        {
                            StartCoroutine(ReturnToDoorI());
                        }
                    }


                }
                else if (playerKeyword.Contains("storage", StringComparison.CurrentCultureIgnoreCase))
                {
                    currentPosition = storage;
                    agent.SetDestination(storageStopPoint.position);
                    if (agent.remainingDistance > 0.1)
                        mainCamera.LookAt = storage;
                    if (agent.remainingDistance <= 0.1 && mainCamera.LookAt == storage)
                    {
                        playerKeyword = "";
                        visitedPoint.Add(storageStopPoint);
                        chatSystem.AddChat("PixelPioneer23 : Few empty Soda Can");
                        chatSystem.AddChat("PixelPioneer23 : but I see something inside the storage");
                        StartCoroutine(ChatDelay(2f, "PixelPioneer23 : We need something thin to wedge it open like a <b>Coin</b> "));
                        if (visitedPoint.Count == 3 && !drawer.activeInHierarchy)
                        {
                            StartCoroutine(ReturnToDoorI());
                        }
                    }
                }
                else if (playerKeyword.Contains("trash", StringComparison.CurrentCultureIgnoreCase))
                {
                    currentPosition = trash;
                    agent.SetDestination(trashStopPoint.position);
                    if (agent.remainingDistance > 0.1 )
                        mainCamera.LookAt = trash;
                    if (agent.remainingDistance <= 0.1 && mainCamera.LookAt == trash)
                    {
                        visitedPoint.Add(trashStopPoint);
                        playerKeyword = "";
                        StartCoroutine(ChatDelay(1f, "TwilightPulse55: Nothing interesting than few boxes "));
                        StartCoroutine(ChatDelay(1.6f, "LunarWanderer19: There is Power Switch though but doubt it will work"));
                        if (visitedPoint.Count == 3 && !drawer.activeInHierarchy)
                        {
                            StartCoroutine(ReturnToDoorI());
                        }
                    }

                }
                else if (playerKeyword.Contains("coin", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!haveCoin && visitedPoint.Contains(shelveStopPoint) && mainCamera.LookAt == shelve)
                    {
                        mainCamera.LookAt = coin;
                        haveCoin = true;
                        Debug.Log("have coin");
                        ResetLookAtI();
                        playerKeyword = "";
                    }
                    else if (haveCoin && visitedPoint.Contains(storageStopPoint))
                    {
                        drawer.SetActive(false);
                        playerKeyword = "";
                        StartCoroutine(ChatDelay(.5f, "TwilightPulse55: More can who would have guess"));
                    }
                }
                else if (playerKeyword.Contains("Door", StringComparison.CurrentCultureIgnoreCase))
                {
                    StartCoroutine(ReturnToDoorI());
                    //codeDoor.eulerAngles = new Vector3(codeDoor.eulerAngles.x, codeDoor.eulerAngles.y, 100);

                }
                else if (playerKeyword.Contains("6459", StringComparison.CurrentCultureIgnoreCase))
                {
                    Debug.Log(currentPosition == codeDoor);
                    playerKeyword = "";
                    if (currentPosition == codeDoor)
                    {
                        if (mainCamera.LookAt != null)
                            mainCamera.LookAt = null;
                        mainCamera.transform.eulerAngles = new Vector3(mainCamera.transform.eulerAngles.x, mainCamera.transform.eulerAngles.z+180, mainCamera.transform.eulerAngles.z);
                        StartCoroutine(OpenDoorDelayI());
                    }
                }else if (playerKeyword.Contains("Back", StringComparison.CurrentCultureIgnoreCase))
                {
                    playerKeyword = "";
                    StartCoroutine(GoBack());
                }
                break;
            case Act.III:
                //if (mainCamera.LookAt == table)
                //    Debug.Log(agent.remainingDistance);
                OnActIIIStart?.Invoke(this, EventArgs.Empty);
                if (playerKeyword.Contains("crib", StringComparison.CurrentCultureIgnoreCase))
                {
                    playerKeyword = "";
                    OnCrib?.Invoke(this, EventArgs.Empty);
                    if (OnCrib == null)
                        Crib();
                }
                else if (playerKeyword.Contains("Back", StringComparison.CurrentCultureIgnoreCase))
                {
                    playerKeyword = "";
                    ReturntoReturnPoint();
                }
                else if (playerKeyword.Contains("Locker", StringComparison.CurrentCultureIgnoreCase))
                {
                    playerKeyword = "";
                    OnLocker?.Invoke(this, EventArgs.Empty);
                    if (OnLocker == null)
                        Locker();
                }
                else if (playerKeyword.Contains("Vent", StringComparison.CurrentCultureIgnoreCase))
                {
                    playerKeyword = "";
                    OnVent?.Invoke(this, EventArgs.Empty);
                    if (OnVent == null)
                        Vent();
                }
                else if (playerKeyword.Contains("Box", StringComparison.CurrentCultureIgnoreCase))
                {
                    playerKeyword = "";
                    Box();
                }
                else if (currentPosition == locker && playerKeyword == "7328")
                {
                    playerKeyword = "";
                    StartCoroutine(ChatDelay(1, "TwilightPulse55: A Screwdriver"));
                    if (!haveScrewdriver)
                        haveScrewdriver = true;
                }
                else if (currentPosition == vent && playerKeyword.Contains("screwdriver", StringComparison.CurrentCultureIgnoreCase) && haveScrewdriver)
                {
                    playerKeyword = "";
                    ventWall.SetActive(false);
                }
                else if (currentPosition == exitDoor && playerKeyword == "3824")
                {
                    playerKeyword = "";
                    StartCoroutine(ExitCoroutine());
                }
                else if (playerKeyword.Contains("Door", StringComparison.CurrentCultureIgnoreCase))
                {
                    playerKeyword = "";
                    Door();
                }else if(mainCamera.LookAt == VHS && playerKeyword.Contains("Play", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!morse.isPlaying)
                        morse.Play(); 
                }

                break;
            case Act.IV:
                if (playerKeyword.Contains("Table", StringComparison.CurrentCultureIgnoreCase))
                {
                    playerKeyword = "";
                    Table();
                }else if(playerKeyword.Contains("Door", StringComparison.CurrentCultureIgnoreCase))
                {
                    playerKeyword = "";
                    StartCoroutine(LastDoor());
                }else if(playerKeyword.Contains("7041") && currentPosition == lastDoor)
                {
                    playerKeyword = "";
                    StartCoroutine(LastDoorCoroutine());
                }else if (playerKeyword.Contains("Back", StringComparison.CurrentCultureIgnoreCase))
                {
                    hasBack = true;
                    StartCoroutine(Dead());
                }
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        WalkForward();
        if (isSliding)
        {
            transform.position = Vector3.Lerp(transform.position, SlideStopPoint.position, Time.fixedDeltaTime*5);
            transform.eulerAngles = Vector3.zero;
        }
    }

    void LookUp()
    {
        if (isLookDown == true)
            isLookDown = false;

        if (hasBottom)
        {
            upXRotation -= 30;
            hasBottom = false;
        }

        if (customRotation >= Mathf.Abs(upXRotation))
        {
            customRotation = 0;
            isLookUp = false;
            playerKeyword = " ";
            hasTop = true;
            upXRotation += 30;
            if (currentAct == Act.I)
                chatSystem.AddChat("TwilightPulse55 : There seem to be whole a lot of nothing");
        }
        mainCamera.transform.eulerAngles = new Vector3(mainCamera.transform.eulerAngles.x - rotateSpeed, mainCamera.transform.eulerAngles.y, mainCamera.transform.eulerAngles.z);
        customRotation += rotateSpeed;
    }

    void LookDown()
    {
        if (isLookUp)
            isLookUp = false;

        if (hasTop)
        {
            downXRotation += 10;
            hasTop = false;
        }

        if (customRotation >= Mathf.Abs(downXRotation))
        {
            customRotation = 0;
            keywordList.Add("Key");
            playerKeyword = " ";
            isLookDown = false;
            hasBottom = true;
            chatSystem.AddChat("QuasarQuest88: Wait there is something, under the box");
            StartCoroutine(ChatDelay(1, "LunarWanderer19: Is That a <b>Key</b>? "));
        }
        mainCamera.transform.eulerAngles = new Vector3(mainCamera.transform.eulerAngles.x + rotateSpeed * 2, mainCamera.transform.eulerAngles.y, mainCamera.transform.eulerAngles.z);
        customRotation += rotateSpeed * 2;
    }

    void WalkForward()
    {
        if (isWalkForward && transform.position.z <= (currentZPosition + maxVDistance))
        {
            rb.AddForce(moveSpeed * transform.forward * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    void TakeKey()
    {
        if (!haveKey)
        {
            transform.GetComponent<AudioSource>().clip = voiceOver2;
            transform.GetComponent<AudioSource>().Play();
        }

        if(mainCamera.transform.eulerAngles.x!=0)
            mainCamera.transform.eulerAngles = Vector3.zero;

        haveKey = true;
        if (haveKey)
        {
            
            StartCoroutine(TakeKeyCoroutine());
        }
    }


    public void StringParser(string message)
    {
        foreach (string keyword in keywordList)
        {
            if (message.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
            {

                playerKeyword = keyword;
                Debug.Log(playerKeyword);
                break;
            }
        }

    }

    private IEnumerator GoBack()
    {
        if (mainCamera.LookAt != codeDoor && mainCamera.LookAt != null)
            mainCamera.LookAt = codeDoor;
        agent.SetDestination(codeDoorStopPointI.position);
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1);
        mainCamera.LookAt = trash;
        yield return new WaitForSeconds(.5f);
        mainCamera.LookAt = null;
        currentPosition = backPos;
    }

    void LookCode(object sender, EventArgs e)
    {
        mainCamera.transform.eulerAngles = new(mainCamera.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y + rotateSpeed, mainCamera.transform.eulerAngles.z);
        customRotation += rotateSpeed;
        if (customRotation >= 5)
        {
            
            mainCamera.LookAt = trash;
            chatSystem.AddChat("LunarWanderer19: There is Trash, Shelve and Red Storage");
            //            isLookCode = false;
            OnLookCode -= LookCode;
            customRotation = 0;
        }
    }

    void RotateRight(object sender, EventArgs e)
    {
        StartCoroutine(RotateRightCoroutine());
    }

    private IEnumerator RotateRightCoroutine()
    {
        mainCamera.LookAt = trash;
        yield return new WaitForSeconds(.5f);
        mainCamera.LookAt = null;
    }

    void RotateLeft(object sender, EventArgs e)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - rotateSpeed * 10, transform.eulerAngles.z);
        customRotation += rotateSpeed * 10;
        if (customRotation >= 80)
        {
            OnRotateLeft -= RotateLeft;
        }
    }

    private IEnumerator Move(Transform target)
    {
        yield return new WaitForSeconds(1);
        //agent.updateRotation = false;
        agent.SetDestination(target.position);
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1);
        OnActIIStart?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator RotateDelay(float timer)
    {
        yield return new WaitForSeconds(timer);
        isRightRotate = true;
    }
    private IEnumerator ChatDelay(float timer, string message)
    {
        yield return new WaitForSeconds(timer);
        chatSystem.AddChat(message);
    }

    void ActIChat(object sender, EventArgs e)
    {
        isBegin = true;
        StartCoroutine(ActIChatCoroutine());
        OnGameStart -= ActIChat;
    }
    void ActIIChat(object sender, EventArgs e)
    {
        StartCoroutine(ActIIChatCoroutine());
        OnActIIStart -= ActIIChat;
    }

    void ActIIIChat(object sender, EventArgs e)
    {
        mainCamera.LookAt = null;
        keywordList.Clear();
        StartCoroutine(ActIIIChatCoroutine());
        OnActIIIStart -= ActIIIChat;
    }

    private IEnumerator ActIChatCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        chatSystem.AddChat("TwilightPulse55 : What is this ???");
        yield return new WaitForSeconds(1);
        chatSystem.AddChat("LunarWanderer19: Did everyone got this recommended ?");
        yield return new WaitForSeconds(.8f);
        chatSystem.AddChat("PixelPioneer23 : Yeah I did, what is this stream ?");
        transform.GetComponent<AudioSource>().clip = voiceOver1;
        transform.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2);
        chatSystem.AddChat("TwilightPulse55 : seem interesting");
        yield return new WaitForSeconds(1);
        chatSystem.AddChat("QuasarQuest88 : Wait is this real ?");
        yield return new WaitForSeconds(1);
        chatSystem.AddChat("PixelPioneer23 : well in any case we should look around the room");
        yield return new WaitForSeconds(.5f);
        chatSystem.AddChat("PixelPioneer23 : there seem to be <b>Top</b> and <b>Bottom</b> part of the shelves");
    }

    private IEnumerator PostActIChat()
    {
        yield return new WaitForSeconds(.3f);
        chatSystem.AddChat("PixelPioneer23 : That’s one done ");
        yield return new WaitForSeconds(.5f);
        chatSystem.AddChat("LunarWanderer19: There seem to be more ");
        yield return new WaitForSeconds(.4f);
        chatSystem.AddChat("Get-Sub4123: Free sub just click at the link bellow bit.ly/Free-Sub3213");
    }

    private IEnumerator TakeKeyCoroutine()
    {

        agent.enabled = true;
        transform.eulerAngles = new(transform.eulerAngles.x, transform.eulerAngles.y + rotateSpeed * 10, transform.eulerAngles.z);
        customRotation += rotateSpeed * 10;

        if (customRotation >= 180)
        {
            StartCoroutine(PostActIChat());
            playerKeyword = " ";
            haveKey = false;
            yield return new WaitForSeconds(2f);
            doorI.transform.position = new Vector3(doorI.position.x, doorI.position.y, -100);
            StartCoroutine(Move(codeDoorStopPointI));
            customRotation = 0;
            currentAct = Act.II;
        }
        


    }

    private IEnumerator ActIIChatCoroutine()
    {
        if (!isLookCode)
        {
            keywordList = new List<string>() { "Back", "Shelve", "Trash", "6459", "storage", "Door" };
            isLookCode = true;
            OnActIIStart?.Invoke(this, EventArgs.Empty);
        }

        currentPosition = codeDoor;
        chatSystem.AddChat("TwilightPulse55 : seem to be Code lock with clue on the wall ");
        yield return new WaitForSeconds(1);
        chatSystem.AddChat("PixelPioneer23 : the clue must be pointing to the stuff in the room ");
    }
    private IEnumerator ActIIIChatCoroutine()
    {
        chatSystem.AddChat("TwilightPulse55 : did anyone hearing this ?");
        yield return new WaitForSeconds(1);
        chatSystem.AddChat("PixelPioneer23: Sound like a baby ???????");
        yield return new WaitForSeconds(1.5f);
        chatSystem.AddChat("QuasarQuest88: Nah Hell nah bruh");
        yield return new WaitForSeconds(.5f);
        chatSystem.AddChat("QuasarQuest88: even if this doesn’t real this creepy as heck");
        agent.SetDestination(testStopPointII.position);
        yield return new WaitUntil(() => agent.remainingDistance <= .1f);
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
        yield return new WaitForSeconds(1f);
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionX;
        yield return new WaitForSeconds(.5f);
        actIIIDoor.eulerAngles = new Vector3(actIIIDoor.eulerAngles.x, actIIIDoor.eulerAngles.y, -100);
        yield return new WaitForSeconds(1);
        agent.SetDestination(returnPoint.position);
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1);
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(1f);
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(.75f);
        mainCamera.LookAt = locker;
        yield return new WaitForSeconds(1f);
        mainCamera.LookAt = vent;
        yield return new WaitForSeconds(1f);
        keywordList = new List<string> { "Crib", "Locker", "Box", "Vent", "Door", "7328", "Screwdriver", "Play", "3824" };
        mainCamera.LookAt = crib;
        yield return new WaitForSeconds(.5f);
        mainCamera.LookAt = null;
        chatSystem.AddChat("LunarWanderer19: That’s a baby crib");
        yield return new WaitForSeconds(1f);
        chatSystem.AddChat("QuasarQuest88: Moment of Truth");
    }

    private IEnumerator OpenDoorDelayI()
    {
        yield return new WaitForSeconds(2f);
        agent.SetDestination(testStopPoint.position);
        codeDoor.eulerAngles = new Vector3(codeDoor.eulerAngles.x, codeDoor.eulerAngles.y, 100);
        baby.clip = babyCry;
        baby.Play();
        yield return new WaitForSeconds(3f);
        //yield return new WaitUntil(() => agent.remainingDistance <= 0.01);
        Debug.Log("b");
        //yield return new WaitForSeconds(1);
        //mainCamera.transform.eulerAngles = new Vector3(mainCamera.transform.eulerAngles.x, 0, mainCamera.transform.eulerAngles.z);
        
        currentAct = Act.III;

    }

    private IEnumerator OnFirstCribCoroutine()
    {
        mainCamera.transform.eulerAngles = Vector3.zero;
        agent.SetDestination(cribStopPoint.position);
        morse.clip = morseCode;
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1);
        if (baby.isPlaying)
            baby.Stop();
        if (!morse.isPlaying)
            morse.Play();
        yield return new WaitForSeconds(.3f);
        chatSystem.AddChat("TwilightPulse55 : It just a tape recorder...");
        yield return new WaitForSeconds(.5f);
        chatSystem.AddChat("LunarWanderer19 : well that’s a relief");
        yield return new WaitForSeconds(.5f);
        chatSystem.AddChat("QuasarQuest88: What kind of Saw game is this.");
        yield return new WaitForSeconds(.3f);
        chatSystem.AddChat("TwilightPulse55 : now it’s playing random sound.");
        yield return new WaitForSeconds(1f);
        chatSystem.AddChat("LunarWanderer19 : We can <b>play</b> the sound again it seem");
        chatSystem.AddChat("LunarWanderer19 : let’s go <b>back</b>");
        keywordList.Add("Back");
        yield return new WaitForSeconds(1f);
        chatSystem.AddChat("LunarWanderer19 : Before,I saw <b>Locker</b>");
        yield return new WaitForSeconds(.8f);
        chatSystem.AddChat("LunarWanderer19 :a <b>Pile of Boxes</b>,a <b>Vent</b> and<b> door</b> too");
        yield return new WaitForSeconds(.5f);
        chatSystem.AddChat("LunarWanderer19 : also an  ominous wall writing");
    }
    private IEnumerator OnCribCoroutine()
    {
        mainCamera.LookAt = crib;
        agent.SetDestination(cribStopPoint.position);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1);
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
        yield return new WaitForSeconds(1f);
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionX;
        mainCamera.LookAt = null;
        currentPosition = crib;
        mainCamera.LookAt = VHS;
    }

    private void ResetLookAtI()
    {
        mainCamera.LookAt = trash;
        //mainCamera.transform.localEulerAngles = Vector3.zero;
    }

    private IEnumerator ReturnToDoorI()
    {
        agent.SetDestination(codeDoorStopPointI.position);
        mainCamera.LookAt = codeDoor;
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1);
        currentPosition = codeDoor;
    }

    private void FirstCrib(object sender, EventArgs e)
    {
        StartCoroutine(OnFirstCribCoroutine());
        OnCrib -= FirstCrib;
    }

    private void Crib()
    {
        StartCoroutine(OnCribCoroutine());
    }

    private void ReturntoReturnPoint()
    {
        StartCoroutine(ReturntoReturnPointCoroutine());
    }

    private IEnumerator ReturntoReturnPointCoroutine()
    {
        agent.SetDestination(returnPoint.position);
        mainCamera.LookAt = returnPoint;
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1);
        currentPosition = returnPoint;
        mainCamera.LookAt = crib;
        yield return new WaitForSeconds(.5f);
        mainCamera.LookAt = null;
    }

    private void FirstLocker(object sender, EventArgs e)
    {
        StartCoroutine(LockerFirstCoroutine());
        OnLocker -= FirstLocker;
    }
    private void Locker()
    {
        StartCoroutine(LockerCoroutine());
    }
    private void Vent()
    {
        StartCoroutine(VentCoroutine());
    }
    private void FirstVent(object sender, EventArgs e)
    {
        StartCoroutine(VentFirstCoroutine());
        OnVent -= FirstVent;
    }

    private void Box()
    {
        agent.SetDestination(boxStopPoint.position);
        mainCamera.LookAt = box;
        currentPosition = box;
    }

    private IEnumerator LockerFirstCoroutine()
    {
        agent.SetDestination(lockerStopPoint.position);
        mainCamera.LookAt = locker;
        currentPosition = locker;
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1);
        StartCoroutine(ChatDelay(.5f, "PixelPioneer23 : Lockers but the one in the right have code lock on it"));
    }
    private IEnumerator LockerCoroutine()
    {
        agent.SetDestination(lockerStopPoint.position);
        mainCamera.LookAt = locker;
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1);
        currentPosition = locker;
    }
    private IEnumerator VentFirstCoroutine()
    {
        agent.SetDestination(ventStopPoint.position);
        mainCamera.LookAt = vent;
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => agent.remainingDistance <= 1);
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll ;
        yield return new WaitForSeconds(1f);
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeAll;
        StartCoroutine(ChatDelay(.5f, "LunarWanderer19: A vent but I see something inside"));
        StartCoroutine(ChatDelay(1f, "PixelPioneer23: We need Something to open the screw"));
        currentPosition = vent;
    }
    private IEnumerator VentCoroutine()
    {
        agent.SetDestination(ventStopPoint.position);
        mainCamera.LookAt = vent;
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => agent.remainingDistance <= 1);
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
        yield return new WaitForSeconds(1f);
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionX;
        currentPosition = vent;
    }

    private void Door()
    {
        StartCoroutine(DoorCoroutine());
    }

    private IEnumerator DoorCoroutine()
    {
        agent.SetDestination(exitStopPoint.position);
        mainCamera.LookAt = exitDoor;
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.01);
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        yield return new WaitForSeconds(1);
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePosition;
        currentPosition = exitDoor;
    }

    private void FirstTable(object sender, EventArgs e)
    {
        StartCoroutine(TableCoroutine());
        OnTable -= FirstTable;
    }    
    private void Table()
    {
        StartCoroutine(TableCoroutine());
    }
    private IEnumerator TableCoroutine()
    {
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(.5f);
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeAll;
        agent.SetDestination(TableStopPoint.position);
        mainCamera.LookAt = table;
        yield return new WaitForSeconds(.5f);
        Debug.Log(agent.remainingDistance);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1);
        agent.isStopped = true;
        mainCamera.LookAt = text;
        currentPosition = table;
    }
    private IEnumerator ExitCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        exitDoor.eulerAngles = new Vector3(exitDoor.eulerAngles.x, exitDoor.eulerAngles.y, 100);
        agent.SetDestination(SlideStopPoint.position);
        mainCamera.LookAt = table;
        Debug.Log(agent.speed);
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.21f);
        Debug.Log("a");
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
        yield return new WaitForSeconds(.5f);
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionZ;
        agent.enabled = false;
        isSliding = true;
        yield return new WaitForSeconds(1.5f);
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX;
        isSliding = false;
        agent.enabled = true;
        yield return new WaitForSeconds(.5f);
        leg.SetActive(true);
        //agent.speed /= 2;
        chatSystem.AddChat("TwilightPulse55 : !!!!! ");
        yield return new WaitForSeconds(.4f);
        chatSystem.AddChat("QuasarQuest88 : I’m going to throw up");
        yield return new WaitForSeconds(.5f);
        chatSystem.AddChat("LunarWanderer19: This can’t be happening");
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationX;
        yield return new WaitForSeconds(.5f);
        currentAct = Act.IV;
        keywordList = new List<string> { "Table", "Door", "7041" };
        mainCamera.LookAt = wallRight;
        yield return new WaitForSeconds(1);
        mainCamera.LookAt = lastDoor;
        StartCoroutine(ChatDelay(.5f, "TwilightPulse55 :  Final one, I hope that <b>door</b> is the exit"));
        yield return new WaitForSeconds(1);
        mainCamera.LookAt = table;
        yield return new WaitForSeconds(.5f);
        OnTable?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator LastDoor()
    {
        if (agent.isStopped)
            agent.isStopped = false;
        Debug.Log(agent.speed);
        agent.SetDestination(lastDoorStopPoint.position);
        mainCamera.LookAt = lastDoor;
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1);
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(.5f);
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeAll;
        currentPosition = lastDoor;
    }
    
    private IEnumerator LastDoorCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        lastDoor.eulerAngles = new Vector3(lastDoor.eulerAngles.x, lastDoor.eulerAngles.y, 100);
        keywordList.Add("Back");
        yield return new WaitForSeconds(20f);
        if (!hasBack)
        {
            StartCoroutine(Dead());
        }

    }

    private IEnumerator Dead()
    {
        keywordList.Clear();
        Zombie.SetActive(true);
        yield return new WaitForSeconds(1);
        mainCamera.LookAt = Zombie.transform;
        yield return new WaitForSeconds(1);
        //agent.enabled = false;
        transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
        yield return new WaitForSeconds(.5f);
        staticCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
    }

    private IEnumerator ResetAgent()
    {
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(.5f);
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeAll;
    }
}
