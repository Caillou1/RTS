using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController G;
    public float MoveSpeed = 5;
    public float ZoomSpeed = 1000;
    public RectTransform SelectedAreaRectTransform;
    public Transform SelectorTransform;
    public CollisionDetection SelectorCollisionDetection;

    private HashSet<AEntity> m_SelectableEntities;
    private HashSet<AEntity> m_SelectedEntities;
    protected Camera m_Camera;
    protected Vector3 m_startPosition;
    protected Transform m_Transform;

    protected bool m_HasCapturedEntity;
    protected GameObject m_CapturedEntity;
    
    void Start()
    {
        G = this;
        m_Transform = transform;
        m_Camera = Camera.main;
        SelectorCollisionDetection.TriggerEnterEvent += OnSelectorTriggerEnter;
        SelectorCollisionDetection.TriggerExitEvent += OnSelectorTriggerExit;
        m_SelectableEntities = new HashSet<AEntity>();
        m_SelectedEntities = new HashSet<AEntity>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(m_HasCapturedEntity)
            {
                EndCapture(true);
            }
            else
            {
                StartSelectionUI();
                StartSelection();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            EndCapture(false);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            UpdateSelectionUI();
            UpdateSelection();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            EndSelectionUI();
            EndSelection();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if(m_HasCapturedEntity)
            {
                EndCapture(false);
            }
            else
            {
                MoveSelection();
            }
        }

        if(m_HasCapturedEntity)
        {
            UpdateCapturedEntity();
        }
    }

    void MoveSelection()
    {
        if (m_SelectedEntities.Count > 0)
        {
            if(Physics.Raycast(m_Camera.ScreenPointToRay(Input.mousePosition), out var hitInfo))
            {
                int counter = 0;
                float delta = .5f;
                int maxPerLine = Mathf.Max(1, Mathf.FloorToInt(m_SelectedEntities.Count / Mathf.Log(m_SelectedEntities.Count)));

                Vector3 center = Vector3.zero;

                foreach (AEntity unit in m_SelectedEntities)
                {
                    center += unit.transform.position;
                }

                center /= m_SelectedEntities.Count;
                Vector3 direction = hitInfo.point - center;
                direction.y = 0;
                direction.Normalize();
                
                foreach (AEntity unit in m_SelectedEntities)
                {
                    int lineNumber = counter / maxPerLine;
                    int thisLineNumber = Mathf.Min(maxPerLine, m_SelectedEntities.Count - lineNumber * maxPerLine);
                    
                    float x = delta * (counter % thisLineNumber) - (thisLineNumber - 1) / 2f * delta;
                    float y = -delta * lineNumber;

                    Vector3 pos = new Vector3(hitInfo.point.x + x, hitInfo.point.y, hitInfo.point.z + y);
                    var v = pos - hitInfo.point;
                    v = Quaternion.LookRotation(direction) * v;
                    pos = hitInfo.point + v;
                    unit.Move(pos);
                    counter++;
                }
            }
        }
    }
    
    void LateUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float zoom = -Input.GetAxis("Mouse ScrollWheel");
        Vector3 translation = new Vector3(horizontal, 0, vertical).normalized * (MoveSpeed * Time.deltaTime);
        Vector3 zoomTranslation = new Vector3(0, zoom, 0) * (ZoomSpeed * Time.deltaTime);
        m_Transform.Translate(translation + zoomTranslation);
    }

    void StartSelectionUI()
    {
        m_startPosition = Input.mousePosition;
        SelectedAreaRectTransform.SetPositionAndRotation(m_startPosition, Quaternion.identity);
        SelectedAreaRectTransform.rect.Set(0, 0, 0, 0);
        
    }

    void UpdateSelectionUI()
    {
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        SelectedAreaRectTransform.SetPositionAndRotation(new Vector3(Mathf.Min(m_startPosition.x, mouseX), Mathf.Max(m_startPosition.y, mouseY), 0), Quaternion.identity);
        SelectedAreaRectTransform.sizeDelta = new Vector2(Mathf.Abs(mouseX - m_startPosition.x) , Mathf.Abs(-mouseY + m_startPosition.y));
    }

    void EndSelectionUI()
    {
        SelectedAreaRectTransform.SetPositionAndRotation(new Vector3(-100, 100, 0), Quaternion.identity);
        SelectedAreaRectTransform.sizeDelta = new Vector2(1, 1);
    }

    void StartSelection()
    {
        Ray ray = m_Camera.ScreenPointToRay(m_startPosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Vector3 worldStart = hitInfo.point;
            SelectorTransform.position = new Vector3(worldStart.x, 0, worldStart.z);
        }
    }

    void UpdateSelection()
    {
        Ray rayStart = m_Camera.ScreenPointToRay(m_startPosition);
        Ray rayEnd = m_Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayStart, out var hitInfoStart) && Physics.Raycast(rayEnd, out var hitInfoEnd))
        {
            Vector3 worldStart = hitInfoStart.point;
            Vector3 worldEnd = hitInfoEnd.point;
            Vector3 size = new Vector3(Mathf.Abs(worldEnd.x - worldStart.x), SelectorTransform.localScale.y, Mathf.Abs(worldEnd.z - worldStart.z));
            SelectorTransform.position = new Vector3(Mathf.Min(worldStart.x, worldEnd.x), 0, Mathf.Max(worldStart.z, worldEnd.z)) + (new Vector3(size.x, 0, -size.z) / 2);
            SelectorTransform.localScale = size;
        }
    }

    void EndSelection()
    {
        SelectorTransform.localScale = new Vector3(0, SelectorTransform.localScale.y, 0);
        SelectorTransform.position = new Vector3(0, -1000, 0);
        
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            foreach (AEntity unit in m_SelectedEntities)
            {
                unit.Selectable.enabled = false;
                unit.Selected.enabled = false;
            }

            m_SelectedEntities.Clear();
        }

        foreach(AEntity unit in m_SelectableEntities)
        {
            unit.Selectable.enabled = false;
            unit.Selected.enabled = true;
            m_SelectedEntities.Add(unit);
        }
        m_SelectableEntities.Clear();
    }

    void UpdateCapturedEntity()
    {
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo))
        {
            m_CapturedEntity.transform.position = hitInfo.point;
        }
    }

    public void StartCapture(GameObject Entity)
    {
        if(m_HasCapturedEntity)
        {
            EndCapture(false);
        }
        m_HasCapturedEntity = true;
        m_CapturedEntity = GameObject.Instantiate(Entity);
    }

    void EndCapture(bool Success)
    {
        m_HasCapturedEntity = false;
        if(Success)
        {
            m_CapturedEntity.GetComponent<Collider>().enabled = true;
        }
        else
        {
            Destroy(m_CapturedEntity);
            m_CapturedEntity = null;
        }
    }

    void OnSelectorTriggerEnter(Collider other)
    {
        var entity = other.GetComponent<AEntity>();
        if (entity)
        {
            entity.Selectable.enabled = true;
            m_SelectableEntities.Add(entity);
            Debug.Log(entity.name);
        }
    }

    void OnSelectorTriggerExit(Collider other)
    {
        var entity = other.GetComponent<AEntity>();
        if (entity)
        {
            entity.Selectable.enabled = false;
            m_SelectableEntities.Remove(entity);
        }
    }
}
