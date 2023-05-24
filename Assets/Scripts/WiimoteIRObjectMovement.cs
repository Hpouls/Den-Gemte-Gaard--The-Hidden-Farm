using UnityEngine;
using System.Collections;
using WiimoteApi;

public class WiimoteIRObjectMovement : MonoBehaviour {
    private Wiimote wiimote;    
    public RectTransform ir_pointer;

    // public array for ir sensor calibration
    public RectTransform[] ir_dots;
    // public array for ir sensor calibration  
    public RectTransform[] ir_bb;  

    // public float to change the area of usable 3D space for ir sensor
    public float upScaling; 

    public GameObject AudioManager;

    // public in to change time a player has after leaving an animal object
    public int Buffer = 5; 

    public SoundPlayer soundPlayer;

    // WiimoteAPI code for enabling the Wii remote within Unity
    void Start() { 
        // look for available Wiimotes
        WiimoteManager.FindWiimotes(); 
        // use the first Wiimote found
        wiimote = WiimoteManager.Wiimotes[0]; 
        // turn on the first LED
        wiimote.SendPlayerLED(true, false, false, false); 
        // Set the input data report mode to IR
        wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_IR10_EXT6); 
        // set up the IR camera to track 2 IR dots
        wiimote.SetupIRCamera(IRDataType.BASIC); 
    }

    // WiimoteAPI code for calculating IRSensor data into a vector2
    public Vector2 irSensorData() { 
        float[,] ir = wiimote.Ir.GetProbableSensorBarIR();
        for (int i = 0; i < 2; i++) {

            float x = (float)ir[i, 0] / 1023f;
            float y = (float)ir[i, 1] / 767f;
            if (x == -1 || y == -1) {
                ir_dots[i].anchorMin = new Vector2(0, 0);
                ir_dots[i].anchorMax = new Vector2(0, 0);
            }

            ir_dots[i].anchorMin = new Vector2(x, y);
            ir_dots[i].anchorMax = new Vector2(x, y);

            if (ir[i, 2] != -1) {
                int index = (int)ir[i, 2];
                float xmin = (float)wiimote.Ir.ir[index, 3] / 127f;
                float ymin = (float)wiimote.Ir.ir[index, 4] / 127f;
                float xmax = (float)wiimote.Ir.ir[index, 5] / 127f;
                float ymax = (float)wiimote.Ir.ir[index, 6] / 127f;
                ir_bb[i].anchorMin = new Vector2(xmin, ymin);
                ir_bb[i].anchorMax = new Vector2(xmax, ymax);
            }
        }

        float[] pointer = wiimote.Ir.GetPointingPosition();
        ir_pointer.anchorMin = new Vector2(pointer[0], pointer[1]);
        ir_pointer.anchorMax = new Vector2(pointer[0], pointer[1]);

        return new Vector2(pointer[0], pointer[1]);
    }

    void Update() {
        if (wiimote != null && wiimote.current_ext != ExtensionController.MOTIONPLUS) { // check if the Wii Motion Plus extension is connected                                                          
            int ret;
            do { ret = wiimote.ReadWiimoteData(); // Read the latest data from the Wii remote
            } while (ret > 0); // Keep reading until all available data has been retrieved
            
            // Converts the Vector2 to a Vector3 with z being 0
            Vector2 v2 = irSensorData();
            Vector3 v3 = new Vector3(v2.x, v2.y, 0);

            // used to move a cube("cursor") on screen via v3
            transform.position = v3 * upScaling;

            //Functions to enable and disable rumble on command. Useful when remote is in a rumble loop
            if (Input.GetKeyDown(KeyCode.S)) {
                // Enabled Rumble
                wiimote.RumbleOn = true; 
                // Requests Status Report, encodes Rumble into input report
                wiimote.SendStatusInfoRequest(); 
            }
            if (Input.GetKeyUp(KeyCode.S)) {
                // Disabled Rumble
                wiimote.RumbleOn = false; 
                // Requests Status Report, encodes Rumble into input report
                wiimote.SendStatusInfoRequest(); 
            }
        }
    }

    void OnCollisionStay(Collision collision) {
        // Only check for objects tagged with "Animal"
        if (collision.gameObject.tag == "Animal") { 
            // Enabled Rumble
            wiimote.RumbleOn = true; 
            // Requests Status Report, encodes Rumble into input report
            wiimote.SendStatusInfoRequest(); 

            // Enables the following function if either "A" or "B" is pressed on the wiimote
            if (wiimote.Button.a || wiimote.Button.b) { 
                // Destroys the collided object
                Destroy(collision.gameObject); 
                // Plays a "FoundSound" which is a ding
                AudioManager.GetComponent<AudioManager>().PlayFoundSound(); 

                if (wiimote.RumbleOn) {
                    // Turns off rumble
                    wiimote.RumbleOn = false; 
                    // Sends the information to the Wiimote to turn off rumble
                    wiimote.SendStatusInfoRequest(); 
                }
            }
            // Starts coroutine which continues the previous function
            StartCoroutine(LingerAPress(collision));         
        }
    }

    void OnCollisionExit(Collision collision) {
        // Starts coroutine when leaving an object
        StartCoroutine(LingerRumble(collision));  
    }

    IEnumerator LingerRumble(Collision collision) {
        // wait for 0.1 seconds times the buffered amount
        yield return new WaitForSeconds(0.1f * Buffer); 
        if (collision.gameObject.tag == "Animal") {
            // Disabled Rumble
            wiimote.RumbleOn = false; 
            // Requests Status Report, encodes Rumble into input report
            wiimote.SendStatusInfoRequest(); 
        }
    }

    IEnumerator LingerAPress(Collision collision) {
        // For loop that loops times the buffered amount
        for (int i = 0; i < Buffer; i++) {  
            // Code to be repeated.

            yield return new WaitForSeconds(0.1f); // wait for 0.1 seconds times the amount of loops
            
            //Same code as OnCollisionStay function
            if (wiimote.Button.a || wiimote.Button.b) {

                //UpdateBool();

                Destroy(collision.gameObject);
                AudioManager.GetComponent<AudioManager>().PlayFoundSound(); // ding
                if (wiimote.RumbleOn) {
                    wiimote.RumbleOn = false;
                    wiimote.SendStatusInfoRequest();
                }
                i = Buffer; //Sets the int i to the buffered amount to stop the loop when either "A" or "B"
            }
        }
    }

    
    void OnDestroy() { //Code to diable the wiimote connection to Unity
        /*if (wiimote != null) {
            wiimote.SendPlayerLED(false, false, false, false); // turn off the player LEDs
            WiimoteManager.Cleanup(wiimote); // clean up the Wiimote connection
        }*/
    }

    void FinishedWithWiimotes() { //Code to diable the wiimote connection to Unity from the WiimoteAPI
        foreach (Wiimote remote in WiimoteManager.Wiimotes) {
            WiimoteManager.Cleanup(remote);
        }
    }
}

