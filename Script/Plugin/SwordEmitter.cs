using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordEmitter : MonoBehaviour {

    public enum Type
    {
        OUT,
        IN,
        AROUND,
    }
    public GameObject Sword;
    public Type FlyDir = Type.IN;
    public int SwordCount = 10;
    public float LifeTime = 10;
    public float Delay = 0;
    public float ColorDuration = 3.0f;
    public bool IsColorLoop = true;
    public float Height = 2.0f;
    public float Radius = 5.0f;
    public float RadiusRangeMin = 0;
    public float RadiusRangeMax = 0;
    public float RadiusRangeTimeMin = 0;
    public float RadiusRangeTimeMax = 0;
    public float Speed = 3.0f;
    public float SpeedRangeMin = 0;
    public float SpeedRangeMax = 0;
    public float SpeedRangeTimeMin = 0;
    public float SpeedRangeTimeMax = 0;
    public Vector3 LocalRotSpeed = Vector3.zero;
    public Color[] ChangeColors;

    private class SwordData
    {
        public GameObject obj;
        public Vector3 dir;
        public GameObject quad;
        public float radius;
        public float speed;
        public float speedUpdateTimer;
        public float radiusUpdateTimer;
    }

    private List<SwordData> m_swordList = new List<SwordData>();

    private float m_durationTimer = 0;
    private float m_lifeTimer = 0;
    private float m_delayTimer = 0;

	void InitData()
    {
        CleanList();
        Sword.transform.GetChild(0).transform.localPosition = FlyDir == Type.AROUND ? new Vector3(0, 0, Radius * Random.Range(1, 3)) : Vector3.zero;
        float radiusStep = Mathf.PI * 2 / SwordCount;

        for (int i = 0; i < SwordCount; ++i)
        {
            SwordData curSwordData = new SwordData();
            curSwordData.obj = GameObject.Instantiate(Sword) as GameObject;
            curSwordData.obj.transform.parent = transform;
            curSwordData.obj.SetActive(true);
            Vector3 curPos = Vector3.zero;
            curPos.x = Mathf.Cos(i * radiusStep) * Radius;
            curPos.z = Mathf.Sin(i * radiusStep) * Radius;
            curSwordData.radius = Radius;
            curSwordData.speed = Speed;
			curSwordData.quad = curSwordData.obj.transform.GetChild(0).gameObject;
            if (SpeedRangeTimeMax > SpeedRangeTimeMin && SpeedRangeTimeMin >= 0)
            {
                curSwordData.speedUpdateTimer = Random.Range(SpeedRangeTimeMin, SpeedRangeTimeMax);
            }

            if (RadiusRangeTimeMax > RadiusRangeTimeMin && RadiusRangeTimeMin >= 0)
            {
                curSwordData.radiusUpdateTimer = Random.Range(RadiusRangeTimeMin, RadiusRangeTimeMax);
            }

            if (FlyDir == Type.IN)
            {

                curPos.y = Height;
                curSwordData.dir = (-curPos).normalized;
                curSwordData.obj.transform.localRotation = Quaternion.LookRotation(curPos);
                curSwordData.obj.transform.localPosition = curPos;
                curSwordData.quad.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));

            }
            else if (FlyDir == Type.OUT)
            {
                Vector3 targetPos = curPos;
                targetPos.x = Mathf.Cos(i * radiusStep) * Radius;
                targetPos.z = Mathf.Sin(i * radiusStep) * Radius;
                targetPos.y = Height;
                curSwordData.dir = targetPos.normalized;
                curSwordData.obj.transform.localRotation = Quaternion.LookRotation(targetPos);
                curSwordData.obj.transform.localPosition = curPos;
                curSwordData.quad.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            }
            else if (FlyDir == Type.AROUND)
            {
                Vector3 curDir = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), 0);
                curSwordData.dir = curDir.normalized;
                curSwordData.quad.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan(-curDir.x / curDir.y) * 180 / Mathf.PI + 90.0f);
                curSwordData.obj.transform.localPosition = Vector3.zero;
                curSwordData.obj.transform.Rotate(curSwordData.dir, Random.Range(0, 360));
            }



            m_swordList.Add(curSwordData);
        }
    }
	// Use this for initialization
	void Start () {
       
	}

    void OnEnable()
    {
        m_durationTimer = ColorDuration;
        m_lifeTimer = LifeTime;
        m_delayTimer = Delay;
        if (Delay <= 0)
        {
            InitData();
        }
    }
 
    // Update is called once per frame
	void Update()
    {
        if (m_delayTimer > 0)
        {
            m_delayTimer -= Time.deltaTime;
            if (m_delayTimer <= 0)
            {
                InitData();
            }
            else
            {
                return;
            }
           
        }

        Color curColor = Color.white;
        if (ChangeColors.Length > 0 && ColorDuration > 0)
        {
            curColor = GetSlerpColor(m_durationTimer);
        }
        for (int i = 0, cout = m_swordList.Count; i < cout; ++i)
        {
            SwordData curObj = m_swordList[i];

            if (ChangeColors.Length > 0 && ColorDuration > 0)
            {
                curObj.quad.renderer.material.SetColor("_TintColor", curColor);
            }
            if (curObj.radiusUpdateTimer > 0)
            {
                curObj.radiusUpdateTimer -= Time.deltaTime;
                if (curObj.radiusUpdateTimer <= 0)
                {
                    curObj.radiusUpdateTimer = Random.Range(RadiusRangeTimeMin, RadiusRangeTimeMax);
                    curObj.radius = Random.Range(RadiusRangeTimeMin, RadiusRangeMax) + Radius;
                }
            }

            if (curObj.speedUpdateTimer > 0)
            {
                curObj.speedUpdateTimer -= Time.deltaTime;
                if (curObj.speedUpdateTimer <= 0)
                {
                    curObj.speedUpdateTimer = Random.Range(SpeedRangeMin, SpeedRangeMax);
                    curObj.speed = Random.Range(SpeedRangeTimeMin, SpeedRangeTimeMax) + Speed;
                }
            }

            if (FlyDir == Type.AROUND)
            {
                curObj.obj.transform.Rotate(curObj.dir, Time.deltaTime * curObj.speed * 180 / Mathf.PI);
                curObj.quad.transform.localPosition = new Vector3(0, 0, curObj.radius);
                curObj.quad.transform.Rotate(LocalRotSpeed);
            }
            else
            {
                curObj.obj.transform.localPosition += curObj.dir * Time.deltaTime * curObj.speed;
                curObj.obj.transform.Rotate(LocalRotSpeed);
            }

        }

        if (m_durationTimer > 0)
        {
            m_durationTimer -= Time.deltaTime;
            if (m_durationTimer <= 0)
            {
                if (IsColorLoop)
                {
                    m_durationTimer = ColorDuration;
                }
                else
                {
                    m_durationTimer = 0;
                }
            }
        }

        if (m_lifeTimer > 0)
        {
            m_lifeTimer -= Time.deltaTime;
            if (m_lifeTimer <= 0)
            {
                CleanList();
            }
        }

    }

    void CleanList()
    {
        for (int i = 0, count = m_swordList.Count; i < count; i++)
        {
            GameObject.Destroy(m_swordList[i].obj);
        }
        m_swordList.Clear();
    }

	Color GetSlerpColor(float leftTime)
    {
        if (ChangeColors == null || ChangeColors.Length == 0)
        {
            return Color.white;
        }

        if (ChangeColors.Length == 1)
        {
            return ChangeColors[0];
        }

        float passedTime = (ColorDuration - leftTime) / ColorDuration;
        int index = (int)((ChangeColors.Length - 1) * passedTime);
        if (index == ChangeColors.Length - 1)
        {
            return ChangeColors[ChangeColors.Length - 1];
        }

        float curPrecent = (ColorDuration - leftTime) / (ColorDuration / (ChangeColors.Length - 1)) - index;
        Color retColor = ChangeColors[index];
        retColor.r += (ChangeColors[index + 1].r - ChangeColors[index].r) * curPrecent;
        retColor.g += (ChangeColors[index + 1].g - ChangeColors[index].g) * curPrecent;
        retColor.b += (ChangeColors[index + 1].b - ChangeColors[index].b) * curPrecent;
        retColor.a += (ChangeColors[index + 1].a - ChangeColors[index].a) * curPrecent;
        return retColor;
    }
}
