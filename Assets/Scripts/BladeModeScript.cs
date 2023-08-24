using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using EzySlice;

public class BladeModeScript : MonoBehaviour
{
    public bool m_bBladeMode;
    public Vector3 mVec_ZoomOffSet;
    public float m_fZoomFOV = 15;
    public Transform m_CutBoxTransForm;
    public CinemachineFreeLook m_FreeLookCamera;
    public Material m_CrossMaterial;
    public LayerMask m_LayerMask;

    Animator m_Anim;
    MovementInput m_MoveInput;
    Vector3 mVec_NormalOffSet;
    float m_fNormalFOV;
    CinemachineComposer[] mArray_Composers;
    ParticleSystem[] mArray_Particles;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_CutBoxTransForm.gameObject.SetActive(false);

        m_Anim = GetComponent<Animator>();
        m_MoveInput = GetComponent<MovementInput>();
        m_fNormalFOV = m_FreeLookCamera.m_Lens.FieldOfView;
        mArray_Composers = new CinemachineComposer[3];

        for(int i = 0; i < mArray_Composers.Length; ++i)
            mArray_Composers[i] = m_FreeLookCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();

        mVec_NormalOffSet = mArray_Composers[0].m_TrackedObjectOffset;
        mArray_Particles = m_CutBoxTransForm.GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        Transform obj1 = Camera.main.transform.GetChild(0); // CutBox

        m_Anim.SetFloat("X", Mathf.Clamp(Camera.main.transform.GetChild(0).localPosition.x + 0.3f, -1, 1));
        m_Anim.SetFloat("Y", Mathf.Clamp(Camera.main.transform.GetChild(0).localPosition.y + 0.18f, -1, 1));

        if (Input.GetMouseButtonDown(1)) DoZoom(true);
        if (Input.GetMouseButtonUp(1)) DoZoom(false);

        if (m_bBladeMode)
        {
            this.transform.rotation = Quaternion.Lerp(transform.rotation, Camera.main.transform.rotation, 0.2f);
            Proc_RotationBox();

            if (Input.GetMouseButtonDown(0))
            {
                m_CutBoxTransForm.GetChild(0).DOComplete();
                m_CutBoxTransForm.GetChild(0).DOLocalMoveX(m_CutBoxTransForm.GetChild(0).localPosition.x * -1, 0.05f).SetEase(Ease.OutExpo);    // FirstRef 를 이동시킴으로 애니메이션 효과
                Proc_ShakeCamera();
                Proc_Slice();
            }
        }
    }

    // ** Alph Omega
    void DoZoom(bool bState)
    {
        m_bBladeMode = bState;
        m_Anim.SetBool("BladeMode", m_bBladeMode);

        m_CutBoxTransForm.localEulerAngles = Vector3.zero;
        m_CutBoxTransForm.gameObject.SetActive(bState);

        string strX = bState ? "Horizontal" : "Mouse X";     
        string strY = bState ? "Vertical" : "Mouse Y";

        // m_XAxis m_YAxis name 변경함으로써 Mouse 에 의한 Move 효과 제외
        m_FreeLookCamera.m_XAxis.m_InputAxisName = strX;
        m_FreeLookCamera.m_YAxis.m_InputAxisName = strY;

        float fov = bState ? m_fZoomFOV : m_fNormalFOV;
        Vector3 offset = bState ? mVec_ZoomOffSet : mVec_NormalOffSet;
        float fTimeScale = bState ? 0.2f : 1.0f;

        DOVirtual.Float(Time.deltaTime, fTimeScale, 0.02f, SetTimeScale);
        DOVirtual.Float(m_FreeLookCamera.m_Lens.FieldOfView, fov, 0.1f, FieldOfView);
        DOVirtual.Float(mArray_Composers[0].m_TrackedObjectOffset.x, offset.x, 0.2f, CameraOffSet).SetUpdate(true); // Time.Scale 과 독립적으로 운용

        m_MoveInput.enabled = !bState;

        if(bState != true)
        {
            this.transform.DORotate(new Vector3(0, this.transform.eulerAngles.y, 0), 0.2f);
        }
    }

    void FieldOfView(float fov)
    {
        m_FreeLookCamera.m_Lens.FieldOfView = fov;
    }

    void CameraOffSet(float x)
    {
        for (int i = 0; i < mArray_Composers.Length; ++i)
            mArray_Composers[i].m_TrackedObjectOffset.Set(x, mArray_Composers[i].m_TrackedObjectOffset.y, mArray_Composers[i].m_TrackedObjectOffset.z);
    }

    void SetTimeScale(float time)
    {
        Time.timeScale = time;
        Debug.Log(time);
    }

    void Proc_RotationBox()
    {
        m_CutBoxTransForm.eulerAngles += new Vector3(0, 0, -Input.GetAxis("Mouse X") * 5);
    }

    void Proc_Slice()
    {
        Collider[] Array_Hits = Physics.OverlapBox(m_CutBoxTransForm.position, new Vector3(5.0f, 0.1f, 5), m_CutBoxTransForm.rotation, m_LayerMask);

        if (Array_Hits.Length <= 0) return;

        for(int i = 0; i < Array_Hits.Length; ++i)
        {
            SlicedHull slicehull = Slice_Object(Array_Hits[i].gameObject, m_CrossMaterial);

            bool bChild = false;
            if (Array_Hits[i].gameObject.GetComponent<MeshRenderer>() == null)
                bChild = true;

            if(slicehull != null)
            {
                GameObject objBottom = slicehull.CreateLowerHull(Array_Hits[i].gameObject, m_CrossMaterial, bChild);
                GameObject objTop = slicehull.CreateUpperHull(Array_Hits[i].gameObject, m_CrossMaterial, bChild);

                Proc_AddHullComponents(objBottom);
                Proc_AddHullComponents(objTop);
                Destroy(Array_Hits[i].gameObject);
            }
        }
    }

    SlicedHull Slice_Object(GameObject obj, Material crossSectionMaterial = null)
    {
        if (obj.GetComponent<MeshFilter>() == null) return null;

        return obj.Slice(m_CutBoxTransForm.position, m_CutBoxTransForm.up, crossSectionMaterial);
    }

    void Proc_AddHullComponents(GameObject obj)
    {
        obj.layer = 9;
        Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
        meshCollider.convex = true;

        rb.AddExplosionForce(100, obj.transform.position, 20);
    }

    void Proc_ShakeCamera()
    {
        m_FreeLookCamera.GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        for(int i = 0; i < mArray_Particles.Length; ++i)
        {
            mArray_Particles[i].Play();
        }
    }
}
