using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{
    public float m_fVelocity;
    public float m_fInputX;
    public float m_fInputZ;
    public Vector3 vec_DesiredMoveDirection;
    public bool m_bBlockRotationPlayer;
    public float m_fDesiredRotationSpeed = 0.1f;
    public float m_fSpeed;
    public float m_fAllowPlayerRotation = 0.1f;
    public Camera m_Camera;
    public CharacterController m_CharacterController;
    public bool m_bIsGround;
    public Animator m_Anim;

    [Header("애니메이션 움직임")]
    [Range(0, 1.0f)]
    public float m_fHorizontalAnimSmoothTime = .2f;
    [Range(0, 1.0f)]
    public float m_fVerticalAnimTime = .2f;
    [Range(0, 1.0f)]
    public float m_fStartAnimTime = .3f;
    [Range(0, 1.0f)]
    public float m_fStopAnimTime = .15f;

    private void Start()
    {
        m_Anim = this.GetComponent<Animator>();
        m_Camera = Camera.main;
        m_CharacterController = this.GetComponent<CharacterController>();
    }

    private void Update()
    {
        InputMagnitude();
    }

    void InputMagnitude()
    {
        m_fInputX = Input.GetAxis("Horizontal");
        m_fInputZ = Input.GetAxis("Vertical");

        m_fSpeed = new Vector2(m_fInputX, m_fInputZ).sqrMagnitude;

        if(m_fSpeed > m_fAllowPlayerRotation)
        {
            m_Anim.SetFloat("Blend", m_fSpeed, m_fStopAnimTime, Time.deltaTime);
            PlayerMoveAndRotation();
        }
        else if(m_fSpeed < m_fAllowPlayerRotation)
        {
            m_Anim.SetFloat("Blend", m_fSpeed, m_fStopAnimTime, Time.deltaTime);
        }
    }    

    void PlayerMoveAndRotation()
    {
        m_fInputX = Input.GetAxis("Horizontal");
        m_fInputZ = Input.GetAxis("Vertical");

        Vector3 vecForward = m_Camera.transform.forward;
        Vector3 vecRight = m_Camera.transform.right;

        vecForward.y = 0.0f;
        vecRight.y = 0.0f;

        vecForward.Normalize();
        vecRight.Normalize();

        vec_DesiredMoveDirection = vecForward * m_fInputZ + vecRight * m_fInputX;

        if(m_bBlockRotationPlayer != true)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vec_DesiredMoveDirection), m_fDesiredRotationSpeed);
            m_CharacterController.Move(vec_DesiredMoveDirection * Time.deltaTime * m_fVelocity);
        }
    }
}
