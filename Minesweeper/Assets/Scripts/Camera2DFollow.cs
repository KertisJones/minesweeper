using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float dampingOnScreen = 1;
        public float dampingOffScreen = 1;
        private float dampingCurrent = 1;
        private float timeSinceDampingChange = 1f;
        private bool onScreenLastTick = true;

        public bool lockY = false;
        public bool lockX = false;
		public float yLockValue = 6.75f;
        public float yAddValue = .8f;
        public float xAddValue = 0f;
        public float xLockValue = 0f;

        public float yMinValue = 0f;
        public float yMaxValue = 5f; //For the top of the arena
        public float xMinValue = -1f; //For the left wall
        public float xMaxValue = 999999f; //For the right wall

        private Camera cam;
		 
		float nextTimeToSearch = 0;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        public Vector3 m_CurrentPosition;
        private Vector3 m_CurrentVelocity;
		private float tarY;

        

        // Use this for initialization
        private void Start()
        {
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                target = GameObject.FindGameObjectWithTag("Player").transform;

                m_LastTargetPosition = target.position;
                m_CurrentPosition = transform.position;
                m_CurrentPosition = new Vector3(m_CurrentPosition.x - xAddValue, m_CurrentPosition.y - yAddValue, m_CurrentPosition.z);
                m_OffsetZ = (m_CurrentPosition - target.position).z;
                transform.SetParent(null);  //transform.parent = null;
            }
            if (this.GetComponent<Camera>() != null)
                cam = GetComponent<Camera>();
            else
                cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }


        // Update is called once per frame
        private void FixedUpdate()
        {

			if (target == null) {
				FindPlayer ();
				return;
			}

            //if the target is off screen, you may want to lower damping
            Vector3 screenPoint = cam.WorldToViewportPoint(target.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            timeSinceDampingChange += Time.deltaTime;
            if (onScreen)
            {
                if (!onScreenLastTick)
                    timeSinceDampingChange = 0;
                dampingCurrent = Mathf.Lerp(dampingCurrent, dampingOnScreen, timeSinceDampingChange);
                onScreenLastTick = true;
            }
            else
            {
                if (onScreenLastTick)
                    timeSinceDampingChange = 0;
                dampingCurrent = Mathf.Lerp(dampingCurrent, dampingOffScreen, timeSinceDampingChange);
                onScreenLastTick = false;
            }
            
            float xMoveDelta = (target.position - m_LastTargetPosition).x;       
            float yMoveDelta = (target.position - m_LastTargetPosition).y;

            Vector3 aheadTargetPos = target.position + new Vector3(0, 0, 0) + Vector3.forward*m_OffsetZ;
            Vector3 newPos;
            newPos = Vector3.SmoothDamp(m_CurrentPosition, aheadTargetPos, ref m_CurrentVelocity, dampingCurrent);

            //Lock appropriate axies.
            if (lockX)
                newPos.x = xLockValue;
            if (lockY)
                newPos.y = yLockValue;
            //do not exceed max or min
            if (newPos.y + yAddValue > yMaxValue)
                newPos.y = yMaxValue;
            if (newPos.y + yAddValue < yMinValue)
                newPos.y = yMinValue - yAddValue;
            if (newPos.x + xAddValue > xMaxValue)
                newPos.x = xMaxValue;
            if (newPos.x + xAddValue < xMinValue)
                newPos.x = xMinValue;

            m_CurrentPosition = newPos;
            transform.position = newPos + new Vector3(0, yAddValue, 0);
            if (transform.position.y < yMinValue)
                transform.position = new Vector3(transform.position.x, yMinValue, transform.position.z);
            m_LastTargetPosition = newPos;
        }
        
		void FindPlayer () {
			if (nextTimeToSearch <= Time.time) {
				GameObject searchResult = GameObject.FindGameObjectWithTag ("Player");
				if (searchResult != null)
					target = searchResult.transform;
				nextTimeToSearch = Time.time + 0.5f;
			}
		}
	
    }
}
