using UnityEngine;
using System.Collections;

//[RequireComponent (typeof(SpriteRenderer))]

public class Tiling : MonoBehaviour {

	public float offsetY = 0f;
    public float offsetX = 0f;


    public float tileDistance = 0f; // the offset so that we don't get any weird errors

    // these are used for checking if we need to instantiate stuff
    public bool hasABottomBuddy = false;
	public bool hasATopBuddy = false;

	public bool reverseScale = false;	// used if the object is not tilable

	[HideInInspector] public float spriteHeight = 0f;		// the Height of our element
	private Camera cam;
	private Transform myTransform;

    //leave null to spawn a copy of self
    public GameObject objToSpawn = null;

	void Awake () {
		cam = Camera.main;
		myTransform = transform;
	}

	// Use this for initialization
	void Start () {
        //SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
		spriteHeight = sRenderer.bounds.size.y;
	}
	
	// Update is called once per frame
	void Update () {
		// does it still need buddies? If not do nothing
		if (hasATopBuddy == false || hasABottomBuddy == false) {
            // calculate the cameras extend (half the Height) of what the camera can see in world coordinates
            //float camVerticalExtend = cam.orthographicSize * Screen.width / Screen.height;
            //Debug.Log(camVerticalExtend);
            // calculate the y position where the camera can see the edge of the sprite (element)
            //float edgeVisiblePositionbottom = (myTransform.position.y + spriteHeight / 2);// - camVerticalExtend;
            //float edgeVisiblePositiontop = (myTransform.position.y - spriteHeight / 2);// + camVerticalExtend;

			// checking if we can see the edge of the element and then calling MakeNewBuddy if we can
			if (cam.transform.position.y <= myTransform.position.y - tileDistance && hasABottomBuddy == false)
			{
				MakeNewBuddy (-1);
				hasABottomBuddy = true;
			}
			else if (cam.transform.position.y >= myTransform.position.y + tileDistance && hasATopBuddy == false)
			{
				MakeNewBuddy (1);
				hasATopBuddy = true;
			}
		}
	}

	// a function that creates a buddy on the side required
	void MakeNewBuddy (int bottomOrtop) {
		// calculating the new position for our new buddy
		Vector3 newPosition = new Vector3 (myTransform.position.x + offsetX, myTransform.position.y + spriteHeight * bottomOrtop + offsetY, myTransform.position.z);
        // instantating our new body and storing him in a variable
        GameObject newBuddy = null;
        if (objToSpawn == null)
        {
            newBuddy = Instantiate(this.gameObject, newPosition, myTransform.rotation);
        }
        else
        {
            newBuddy = Instantiate(objToSpawn, newPosition, myTransform.rotation);
        }
		// if not tilable let's reverse the y size og our object to get rid of ugly seams
		if (reverseScale == true) {
			newBuddy.transform.localScale = new Vector3 (newBuddy.transform.localScale.x, newBuddy.transform.localScale.y * -1, newBuddy.transform.localScale.z);
		}

		newBuddy.transform.parent = myTransform.parent;
		if (bottomOrtop < 0) {
			newBuddy.GetComponent<Tiling>().hasATopBuddy = true;
		}
		else {
			newBuddy.GetComponent<Tiling>().hasABottomBuddy = true;
		}
	}
}
