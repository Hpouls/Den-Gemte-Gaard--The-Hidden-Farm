using UnityEngine;
using System.Collections;
using WiimoteApi;

public class WiimoteIRObjectMovement : MonoBehaviour {
    private Wiimote wiimote;    
    public RectTransform ir_pointer;

    public RectTransform[] ir_dots;  //public array for ir sensor calibration
    public RectTransform[] ir_bb;  //public array for ir sensor calibration

    public float upScaling; //public float to change the area of usable 3D space for ir sensor

    public GameObject AudioManager;

    public int Buffer = 5; //public in to change time a player has after leaving an animal object

    //public GameManager gameManager;

    public bool destroySound = false; //Might not do anything, else, the bool is for ensuring that the sound is destroyed

    public SoundPlayer soundPlayer;

    void Start() { //WiimoteAPI code for enabling wiimote within Unity
        WiimoteManager.FindWiimotes(); // look for available Wiimotes
        wiimote = WiimoteManager.Wiimotes[0]; // use the first Wiimote found
        wiimote.SendPlayerLED(true, false, false, false); // turn on the first LED
        wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_IR10_EXT6); // Set the input data report mode to IR
        wiimote.SetupIRCamera(IRDataType.BASIC); // set up the IR camera to track 2 IR dots
    }

    public Vector2 irSensorData() { //More WiimoteAPI code for calculating IRSensor data into a vector2
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

            // used to move a cube on screen for developers to visualize irSensorData
            transform.position = v3 * upScaling;

            //Functions to enable and disable rumble on command. Useful when remote is in a rumble loop
            if (Input.GetKeyDown(KeyCode.S)) {
                wiimote.RumbleOn = true; // Enabled Rumble
                wiimote.SendStatusInfoRequest(); // Requests Status Report, encodes Rumble into input report
            }
            if (Input.GetKeyUp(KeyCode.S)) {
                wiimote.RumbleOn = false; // Disabled Rumble
                wiimote.SendStatusInfoRequest(); // Requests Status Report, encodes Rumble into input report
            }
        }
    }

    /*void UpdateBool() {
        GameObject g = GameObject.FindGameObjectWithTag("GameManager");
        soundPlayer.toggle = g.GetComponent<GameManager>();
        soundPlayer.toggle = true;

        gameObject.GetComponent<SoundPlayer>().PlaySound();
    }*/

    void OnCollisionStay(Collision collision) {
        if (collision.gameObject.tag == "Animal") { //Only check for objects tagged with "Animal"
            wiimote.RumbleOn = true; // Enabled Rumble
            wiimote.SendStatusInfoRequest(); // Requests Status Report, encodes Rumble into input report

            if (wiimote.Button.a || wiimote.Button.b) { //Enables the following function if either "A" or "B" is pressed on the wiimote
                //Was used to test functionality
                //Debug.Log("'A' button pressed!!");

                //UpdateBool();

                destroySound = true; //Again, might not do anything, else, tells the program that the sound is indeed destroyed

                Destroy(collision.gameObject); //Destroys the collided object

                AudioManager.GetComponent<AudioManager>().PlayFoundSound(); //Plays a "FoundSound" which is a ding

                if (wiimote.RumbleOn) {
                    wiimote.RumbleOn = false; //Turns off rumble
                    wiimote.SendStatusInfoRequest(); //Sends the information to the Wiimote to turn off rumble
                }
            }
            StartCoroutine(LingerAPress(collision)); //Starts coroutine which continues the previous function        
        }
    }

    void OnCollisionExit(Collision collision) {
        StartCoroutine(LingerRumble(collision)); //Starts coroutine when leaving an object 
    }

    IEnumerator LingerRumble(Collision collision) {
        yield return new WaitForSeconds(0.1f * Buffer); // wait for 0.1 seconds times the buffered amount
        if (collision.gameObject.tag == "Animal") {
            wiimote.RumbleOn = false; // Disabled Rumble
            wiimote.SendStatusInfoRequest(); // Requests Status Report, encodes Rumble into input report
        }
    }

    IEnumerator LingerAPress(Collision collision){
        for (int i = 0; i < Buffer; i++) { //For loop that loops times the buffered amount 
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

