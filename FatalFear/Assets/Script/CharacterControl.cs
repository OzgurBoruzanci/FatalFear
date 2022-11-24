using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    int bulletPoolCounter = 0;
    float horizontal=0, vertical=0;
    float headRotUpDown=0, headRotLefRight=0;
    public float speed = 0;

    bool shotControl = false;

    Coroutine memoryCoroutine=null;
    Transform skeleton;
    Rigidbody physics;
    Animator animator;
    GameObject mainCamera, pos1, pos2;
    public GameObject headCamera;
    public GameObject sight;
    public Vector3 offset;
    public RuntimeAnimatorController onShot;
    public RuntimeAnimatorController notShot;
    public GameObject bullet;
    public Transform bulletPosition;
    public GameObject[] bulletPool;

    RaycastHit hit;
    RaycastHit hitFire;
    Vector3 distanceBetweenCamera;


    void Start()
    {
        animator = GetComponent<Animator>();
        physics = GetComponent<Rigidbody>();
        distanceBetweenCamera = headCamera.transform.position - transform.position;
        mainCamera = Camera.main.gameObject;
        pos1 = headCamera.transform.Find("Pos1").gameObject;
        pos2 = headCamera.transform.Find("Pos2").gameObject;

        skeleton = animator.GetBoneTransform(HumanBodyBones.Chest);

        BulletPool();
    }

    void BulletPool()
    {
        bulletPool=new GameObject[10];
        for (int i = 0; i < bulletPool.Length; i++)
        {
            GameObject bulletObj = Instantiate(bullet);
            bulletObj.SetActive(false);
            bulletPool[i]=bulletObj;
        }
    }

    private void LateUpdate()
    {
        if (shotControl)
        {
            if (hitFire.distance>3)
            {
                skeleton.LookAt(hitFire.point);
                skeleton.rotation *= Quaternion.Euler(offset);
            }
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("JumpParameters",true);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!shotControl)
            {
                speed *= 2;
            }
            animator.SetBool("RunParameters", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (!shotControl)
            {
                speed/=2;
            }
            animator.SetBool("RunParameters", false);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            sight.SetActive(true);
            shotControl = true;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            sight.SetActive(false);
            shotControl = false;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (memoryCoroutine!=null)
            {
                StopCoroutine(memoryCoroutine);
            }
            animator.SetBool("FireParameters", true);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            memoryCoroutine = StartCoroutine(FireAnimStop());
        }
    }
    IEnumerator FireAnimStop()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("FireParameters", false);
    }

    void Shot()
    {
        bulletPool[bulletPoolCounter].SetActive(true);
        bulletPool[bulletPoolCounter].transform.position = bulletPosition.position;
        bulletPool[bulletPoolCounter].transform.rotation = bulletPosition.rotation;

        Rigidbody bulletPhysics = bulletPool[bulletPoolCounter].GetComponent<Rigidbody>();
        bulletPhysics.velocity = Vector2.zero;
        bulletPhysics.AddForce((hitFire.point - bulletPosition.transform.position).normalized * 1000);
        bulletPoolCounter++;
        if (bulletPoolCounter == bulletPool.Length)
        {
            bulletPoolCounter = 0;
        }
    }

    void FixedUpdate()
    {
        Move();
        if (!shotControl)
        {
            animator.runtimeAnimatorController = notShot;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, pos1.transform.position, 0.1f);
            RotationCamera();
        }
        else
        {
            animator.runtimeAnimatorController = onShot;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, pos2.transform.position, 0.1f);
            RotationCamera2();
        }
        
        
        animator.SetFloat("HorizontalParameters", horizontal);
        animator.SetFloat("VerticalParameters", vertical);
    }
    void Move()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector3 vec = new Vector3(horizontal, 0, vertical);
        vec=transform.TransformDirection(vec);
        vec.Normalize();
        transform.position += vec*Time.deltaTime*2;

    }
    void RotationCamera2()
    {
        RotationMouseControl();

        Physics.Raycast(Vector3.zero, headCamera.transform.GetChild(0).forward, out hit);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(hit.point.x, 0, hit.point.z)), 0.4f);

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Physics.Raycast(ray, out hitFire);
    }
    void RotationCamera()
    {
        RotationMouseControl();

        if (horizontal != 0 || vertical != 0)
        {
            Physics.Raycast(Vector3.zero, headCamera.transform.GetChild(0).forward, out hit);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(hit.point.x, 0, hit.point.z)), 0.4f);

        }
    }

    void RotationMouseControl()
    {
        headCamera.transform.position = transform.position + distanceBetweenCamera;
        headRotUpDown += Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * -150;
        headRotLefRight += Input.GetAxis("Mouse X") * Time.fixedDeltaTime * 150;
        headRotUpDown = Mathf.Clamp(headRotUpDown, -20, 20);
        headCamera.transform.rotation = Quaternion.Euler(headRotUpDown, headRotLefRight, transform.eulerAngles.z);
    }

    void JumpDownFalse()
    {
        animator.SetBool("JumpParameters", false);
    }
    void JumpAddForce()
    {
        physics.AddForce(0, Time.deltaTime * 10000, 0);
    }
}
