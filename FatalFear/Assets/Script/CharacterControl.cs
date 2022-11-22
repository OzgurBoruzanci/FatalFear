using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    float horizontal=0, vertical=0;
    float headRotUpDown=0, headRotLefRight=0;
    public float speed = 0;

    bool shootControl = false;

    Transform skeleton;
    Rigidbody physics;
    Animator animator;
    GameObject mainCamera, pos1, pos2;
    public GameObject headCamera;
    public GameObject sight;
    public Vector3 offset;

    RaycastHit hit;
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
    }

    private void LateUpdate()
    {
        skeleton.rotation *= Quaternion.Euler(offset);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("JumpParameters",true);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= 2;
            animator.SetBool("RunParameters", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed/=2;
            animator.SetBool("RunParameters", false);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            sight.SetActive(true);
            shootControl = true;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            sight.SetActive(false);
            shootControl = false;
        }
    }


    void FixedUpdate()
    {
        Move();
        if (!shootControl)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, pos1.transform.position, 0.1f);
            RotationCamera();
        }
        else
        {
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
