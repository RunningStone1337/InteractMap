using System;
using UnityEngine;

namespace Scene
{
    public class MainCamera : MonoBehaviour
    {
        #region EVENTS

        //public delegate void MainCameraEventHandler(object sender, MainCameraEventArgs args);

        public event Action<float> OnCameraSizeChangedEvent;

        public event Action OnCameraSwipeEndEvent;

        public event Action OnCameraSwipeStartEvent;

        #endregion EVENTS

        #region PUBLIC VARIABLES

        public Vector2 CameraStartPos
        {
            get { return cameraStartPos; }
            set { cameraStartPos = value; }
        }

        public float DownBound
        { get { return downBound; } set { downBound = value; } }

        public bool FreezeSwipes { get => freezeSwipes; set => freezeSwipes = value; }

        public bool IsCameraEventContinued { get; private set; }
        public bool IsZoomPermissed { get; internal set; } = true;

        public float LeftBound
        { get { return leftBound; } set { leftBound = value; } }

        public float MaxZoomOnGlobal
        { get { return maxZoomOnGlobal; } }

        public float MinZoomOnGlobal
        { get { return minZoomOnGlobal; } }

        public Vector3 PointerStartPos { get; private set; }

        public float RightBound
        { get { return rightBound; } set { rightBound = value; } }

        public float UpBound
        { get { return upBound; } set { upBound = value; } }

        #endregion PUBLIC VARIABLES

        #region SERIALIZE FIELDS

        [SerializeField] public Camera mainCam;

        [Header("Текущие значения")]
       
        [SerializeField] private float maxZoomOnGlobal = 40f;
        [SerializeField] private float minZoomOnGlobal = 250f;
        [SerializeField] private float wheelSpeed = 70f;
        [SerializeField] private float leftBound = float.MinValue;
        [SerializeField] private float rightBound = float.MaxValue;
        [SerializeField] private float upBound = float.MaxValue;
        [SerializeField] private float downBound = float.MinValue;
        [SerializeField] private bool freezeSwipes;

        #endregion SERIALIZE FIELDS

        #region PRIVATE VARIABLES

        private const float blue = 0.07843138f;
        private const float green = 0.254902f;
        private const float red = 0.05882353f;
        private Vector2 cameraStartPos;

        #endregion PRIVATE VARIABLES
       
        #region PRIVATE METHODS

        private void Awake()
        {
            if (!mainCam)
                mainCam = gameObject.GetComponent<Camera>();
            mainCam.orthographicSize = 30f;
            mainCam.farClipPlane = 1000f;
            mainCam.nearClipPlane = 0.3f;
            mainCam.backgroundColor = new Color(red, green, blue, 0);
        }

        private void MouseScroll(float scroll)
        {
            if (scroll != 0.0f && IsZoomPermissed)
            {
                mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize - scroll * wheelSpeed, MaxZoomOnGlobal, MinZoomOnGlobal);
                SetCamPosOnBoundsConstraints();
                OnCameraSizeChangedEvent?.Invoke(mainCam.orthographicSize);
            }
        }

        private void OnCameraSizeChanged(float value)
        {
            IsCameraEventContinued = false;
        }

        private void OnCameraSwiped()
        {
            IsCameraEventContinued = false;
        }

        private void OnDestroy()
        {
            OnCameraSizeChangedEvent -= OnCameraSizeChanged;
            OnCameraSwipeStartEvent -= OnCameraSwiped;
        }

        private void SetCamPosOnBoundsConstraints()
        {
            var posX = gameObject.transform.position.x;
            var posY = gameObject.transform.position.y;
            transform.position = new Vector3(Mathf.Clamp(posX, LeftBound, RightBound), Mathf.Clamp(posY, DownBound, UpBound), -100f);
        }

        private void Start()
        {
            OnCameraSizeChangedEvent += OnCameraSizeChanged;
            OnCameraSwipeStartEvent += OnCameraSwiped;
        }

        private void Swipes()
        {
            if (!FreezeSwipes)
            {
                if (Input.GetMouseButtonDown(0))//если нажата левая
                {
                    CameraStartPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                    PointerStartPos = Input.mousePosition;
                }
                else if (Input.GetMouseButton(0))//если нажата и удерживается
                {
                    var mPos = Input.mousePosition;
                    var posX = mainCam.ScreenToWorldPoint(mPos).x - CameraStartPos.x;
                    var posY = mainCam.ScreenToWorldPoint(mPos).y - CameraStartPos.y;
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x - posX, leftBound, rightBound),
                        Mathf.Clamp(transform.position.y - posY, downBound, upBound), -100f);
                    SetCamPosOnBoundsConstraints();
                    if (Mathf.Abs(PointerStartPos.x - mPos.x) > 1f || Mathf.Abs(PointerStartPos.y - mPos.y) > 1f)//если свайп сильнее 1
                        OnCameraSwipeStartEvent?.Invoke();
                    else
                        OnCameraSwipeEndEvent?.Invoke();
                }
            }
        }

        private void Update()
        {
            /// Зум камеры мышью
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            MouseScroll(scroll);
            Swipes();
        }

        #endregion PRIVATE METHODS
    }
}