using UnityEngine;
using UnityEngine.Networking ;


[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot: NetworkBehaviour {

    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera cam;

    private PlayerWeapon curretnWeapon;

    private WeaponManager weaponManager;

    [SerializeField]
    private LayerMask mask;


    private void Start() {
        if (cam == null) {
            Debug.LogError("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();

    }

    private void Update() {
        curretnWeapon = weaponManager.GetCurretnWeapon();


        if (curretnWeapon.fireRate <= 0f) {
            if (Input.GetButtonDown("Fire1")) {
                Shoot();
            }
        }
        else {
            if (Input.GetButtonDown("Fire1")) {
                InvokeRepeating("Shoot", 0f, 1f / curretnWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1")) {
                CancelInvoke("Shoot");
            }
        }
    }

    [Client]
    void Shoot() {
        RaycastHit _hit;

        Debug.Log("Shoot");

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, curretnWeapon.range, mask)) {
            if (_hit.collider.tag == PLAYER_TAG) {
                CmdPlayerShot(_hit.collider.name, curretnWeapon.damage);
            }
        }

    }

    [Command]
    void CmdPlayerShot(string _playerID, int _damage) {
        Debug.Log(_playerID + " has been shot");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);

//        Destroy(GameObject.Find(_ID));
    }

}
