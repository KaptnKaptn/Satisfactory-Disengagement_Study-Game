using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//player jump input handler
public class PlayerJumpInput : MonoBehaviour
{
    #region Touch Components
    private RectTransform touchArea;
    private Touch touch;
    private int trackedFingerID;
    private Player_Joystick player;
    #endregion

    public bool charging;

    // Start is called before the first frame update
    void Start()
    {
        touchArea = this.GetComponent<RectTransform>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Joystick>();
    }

    // Update is called once per frame
    void Update()
    {
        #region Jump Touch Check
        if (!charging)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.touches[Input.touches.Length - 1];
                Debug.Log("Update: " + Input.touches[Input.touches.Length - 1].fingerId);
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(touchArea, touch.position)
                && !charging)
            {
                trackedFingerID = touch.fingerId;
                TouchCharge();
            }
        }
        #endregion

        #region Jump Release
        else
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                Touch currentTouch = Input.GetTouch(i);
                if (currentTouch.fingerId == trackedFingerID &&
                    currentTouch.phase == TouchPhase.Ended)
                {
                    TouchRelease();
                }
            }
        }
        #endregion
    }

    private void TouchCharge()
    {
        charging = true;
        player.JumpButton();
    }

    private void TouchRelease()
    {
        charging = false;
        touch.position = new Vector2(-10, -10);
        player.JumpRelease();
    }


}
