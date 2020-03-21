/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DOFprojFPS
{
    public class Weapon : MonoBehaviour
    {
        //Firearms settings
        public WeaponSettingSO weaponSetting;
        public string weaponName;
        public WeaponType weaponType;
        
        public int ammoItemID;
        
        public Transform muzzleFlashTransform;
        public Transform shellTransform;
        
        public float reloadAnimationDuration = 3.0f;
        public bool autoReload = true;
        public int maxAmmo;
        
        public enum FireMode { automatic, single}
        public FireMode fireMode;

        //Referenced weapon item 
        public Item currentItem;

        //Special variables for rocket launcher. When we shot a missile we actualy push prefab.
        //We should hide missile representation on visible on weapon object and set active after some time elapsed
        public GameObject MissileObject;
        public float timeToShowMissileObject;
        
        //Grenade active prefab
        public GameObject grenadePrefab;

        #region Utility variables

        //Here is a utility variables which get value from other scripts, set values to other scripts or used for calculations etc. 
        //I will not comment them because it may look complicated and dirty
        //If you'll get questions about them write me on email please

        [HideInInspector]
        public FPSController controller;
        
        private ParticleSystem MuzzleFlashParticlesFX;
        private AudioClip shotSFX, reloadingSFX, emptySFX;
        private GameObject shell;
        private int shellPoolSize;
        private float shellForce;
        private Text ammoText;
        private Text weaponNameText;

        //Stats
        [HideInInspector]
        public int damageMin, damageMax;
        private float fireRate;
        [HideInInspector]
        public float rigidbodyHitForce;
        private float spread;
        private bool useScope;
        private float scopeFOV;

        [HideInInspector]
        public Animator animator;
        private AudioSource audioSource;
        private Camera cam;

        private GameObject[] decals;
        private GameObject[] shells;

        private int shellIndex = 0;

        private int decalIndex_wood = 0;
        private int decalIndex_concrete = 0;
        private int decalIndex_dirt = 0;
        private int decalIndex_metal = 0;

        private float nextFireTime;

        [HideInInspector]
        public bool reloading = false;
        [HideInInspector]
        public bool canShot = true;
        [HideInInspector]
        public bool setAim = false;
        
        private float normalFOV;
        private float normalSensX;
        private float normalSensY;

        private float scopeSensitivityX, scopeSensitivityY;

        private ParticleSystem temp_MuzzleFlashParticlesFX;

        private WeaponManager weaponManager;
        
        private GameObject projectile;
        private GameObject[] projectiles;
        private int projectilePoolSize;
        private int projectilesIndex;

        private Vector2 recoil;

        private HitFXManager hitFXManager;

        [HideInInspector]
        public int calculatedDamage;

        private AudioSource ricochetSource;
        private AudioClip[] ricochetSounds;
        
        //Melee variables (used only if weapon is melee type)
        private float meleeAttackDistance;
        private float meleeAttackRate;
        private int meleeDamagePoints;
        private float meleeRigidbodyHitForce;
        private float meleeHitTime;
        private AudioClip meleeHitFX;

        //This factor used for adding more spread when character shooting in movement
        private float movementSpreadFactor;

        private float bulletInitialVelocity;
        private float airResistanceForce;

        private DTInventory inventory;
        private InputManager input;
        private Sway sway;
        
        private GameObject reticle;

        [HideInInspector]
        public bool isThrowingGrenade = false;

        private Animator weaponHolderAnimator;
        #endregion


        // Get weapon name on Awake() before WeaponManager will disable weapon components. Made for pickup functionality. WeaponManager enables weapon by name and weapon name must be initialized before it turns off
        private void Awake()
        {
            weaponName = weaponSetting.weaponName;
        }

        //Main setting function which takes values from Weapon scriptable object and set it to this class instance
        private void GetWeaponSettings()
        {
            weaponType = weaponSetting.weaponType;

            if (weaponType != WeaponType.Melee)
                MuzzleFlashParticlesFX = weaponSetting.MuzzleFlashParticlesFX;

            shotSFX = weaponSetting.shotSFX;
            reloadingSFX = weaponSetting.reloadingSFX;
            emptySFX = weaponSetting.emptySFX;

            shell = weaponSetting.shell;
            shellPoolSize = weaponSetting.shellsPoolSize;
            shellForce = weaponSetting.shellsPoolSize;

            damageMin = weaponSetting.damageMinimum;
            damageMax = weaponSetting.damageMaximum;
            fireRate = weaponSetting.fireRate;
            rigidbodyHitForce = weaponSetting.rigidBodyHitForce;
            spread = weaponSetting.spread;
            useScope = weaponSetting.canUseScope;
            scopeFOV = weaponSetting.scopeFOV;
            scopeSensitivityX = weaponSetting.scopeSensitivityX;
            scopeSensitivityY = weaponSetting.scopeSensitivityY;

            bulletInitialVelocity = weaponSetting.bulletInitialVelocity;
            airResistanceForce = weaponSetting.airResistanceForce;

            projectile = weaponSetting.projectile;
            projectilePoolSize = weaponSetting.projectilePoolSize;

            recoil = weaponSetting.recoil;

            if (weaponType == WeaponType.Melee)
            {
                meleeAttackDistance = weaponSetting.meleeAttackDistance;
                meleeAttackRate = weaponSetting.meleeAttackRate;
                meleeDamagePoints = weaponSetting.meleeDamagePoints;
                meleeRigidbodyHitForce = weaponSetting.meleeRigidbodyHitForce;
                meleeHitTime = weaponSetting.meleeHitTime;
                meleeHitFX = weaponSetting.meleeHitFX;
            }

    }
        
        private void Start()
        {
            GetWeaponSettings();

            if (weaponType != WeaponType.Melee && weaponType != WeaponType.Grenade && weaponType != WeaponType.RocketLauncher)
                BalisticProjectilesPool();

            if (GetComponent<Animator>())
                animator = GetComponent<Animator>();
            else
                Debug.LogError("Please attach animator to your weapon object");

            if (GetComponent<AudioSource>())
                audioSource = GetComponentInParent<AudioSource>();
            else
                Debug.LogError("Please attach AudioSource to your weapon object");

            cam = Camera.main;

            controller = FindObjectOfType<FPSController>();

            normalSensX = controller.sensitivity.x;
            normalSensY = controller.sensitivity.y;
            normalFOV = cam.fieldOfView;

            weaponManager = FindObjectOfType<WeaponManager>();
            
            ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
            weaponNameText = GameObject.Find("WeaponText").GetComponent<Text>();
            
            sway = FindObjectOfType<Sway>();

            reticle = FindObjectOfType<ReticleAnimation>().gameObject;
            
            if (weaponType != WeaponType.Melee && weaponType != WeaponType.Grenade)
            {
                if (weaponType != WeaponType.RocketLauncher)
                {
                    if (shell)
                        ShellsPool();
                    else
                        Debug.LogError("No shell gameobject attached to weapon settings object");
                }
                if (MuzzleFlashParticlesFX)
                {
                    temp_MuzzleFlashParticlesFX = Instantiate(MuzzleFlashParticlesFX, muzzleFlashTransform.position, muzzleFlashTransform.rotation, muzzleFlashTransform);
                    temp_MuzzleFlashParticlesFX.transform.parent = transform;// GameObject.Find("GamePrefab").transform;
                }
                else
                    Debug.LogWarning("There is no shot particle system attached to weapon settings");

            }

            hitFXManager = FindObjectOfType<HitFXManager>();

            ricochetSource = hitFXManager.gameObject.GetComponent<AudioSource>();
            ricochetSounds = hitFXManager.ricochetSounds;

            weaponHolderAnimator = GameObject.Find("Weapon holder").GetComponent<Animator>();

            inventory = FindObjectOfType<DTInventory>();
            input = FindObjectOfType<InputManager>();
        }


        void Update()
        {
            if(weaponType != WeaponType.RocketLauncher) movementSpreadFactor = controller.GetVelocityMagnitude();

            if (Input.GetKey(input.Fire) &&!InputManager.useMobileInput && Time.timeScale != 0 && !PlayerStats.isPlayerDead && weaponType != WeaponType.RocketLauncher  && weaponType != WeaponType.Pistol && !InventoryManager.showInventory && fireMode == FireMode.automatic)  //Statement to restrict auto-fire for pistol weapon type. Riffle and others are automatic
            {
                Fire();
            }
            else if (Input.GetKeyDown(input.Fire) && !InputManager.useMobileInput && Time.timeScale != 0 && !PlayerStats.isPlayerDead && weaponType != WeaponType.RocketLauncher && (weaponType == WeaponType.Pistol || fireMode == FireMode.single) && !InventoryManager.showInventory)
            {
                Fire();
            }
            else if(Input.GetKeyDown(input.Fire) && !InputManager.useMobileInput && Time.timeScale != 0 && !PlayerStats.isPlayerDead && weaponType == WeaponType.RocketLauncher && !InventoryManager.showInventory)
            {
                FireRocketLauncher();
            }

            if (weaponType != WeaponType.Melee && weaponType != WeaponType.Grenade)
            {
                //Reloading consists of two stages ReloadBegin and ReloadEnd  
                //ReloadBegin method play animation and soundFX and also restrict weapon shooting. ReloadingEnd removes restriction and add ammo to weapon
                //See more in methods below
                if (Input.GetKeyDown(input.Reload) || currentItem.weaponAmmoCount == 0)
                {
                    if (weaponType != WeaponType.Shotgun)
                    {
                        if (!reloading && !controller.isClimbing && currentItem.weaponAmmoCount < maxAmmo)
                            ReloadBegin();
                    }
                    else
                    {
                        if(!reloading && !controller.isClimbing && currentItem.weaponAmmoCount < maxAmmo)
                        ShotgunReload();
                    }
                }

                //setAim variable used for controlling states in SetAim() method
                if (!InputManager.useMobileInput)
                {
                    if (Input.GetKey(input.Aim))
                    {
                        setAim = true;
                        reticle.SetActive(false);
                        sway.xSwayAmount = 0;//*0.3f;
                        sway.ySwayAmount = 0;//*0.3f;
                    }
                    else
                    {
                        setAim = false;

                        if(!reloading)
                            reticle.SetActive(true);

                        sway.xSwayAmount = sway.startX;
                        sway.ySwayAmount = sway.startY;
                    }
                }

                SetAim();
            }

            //Special instructions for shotgun!
            if (weaponType == WeaponType.Shotgun)
            {
                if(reloading && CalculateTotalAmmo() == 0)
                {
                    animator.SetBool("Reloading", false);
                }

                if (Input.GetKeyDown(input.Fire) && reloading)
                {
                    animator.SetBool("Reloading", false);
                }

                if (currentItem.weaponAmmoCount == maxAmmo)
                {
                    animator.SetBool("Reloading", false);
                }
            }

            UpdateAmmoText();

            if(animator.GetBool("Use") != false)
                animator.SetBool("Use", UseObjects.useState);
            
            FireModeSwitch();
        }
        
        //Hook for mobile shooting
        public void FireMobile()
        {
            Fire();
        }

        //Hook for mobile aiming
        public void AimMobile()
        {
            setAim = !setAim;
        }

        public void PlayUseAnimation()
        {
            if(animator)
                animator.Play("Take");
        }

        public void Fire()
        {
            if (weaponType != WeaponType.Melee && weaponType != WeaponType.Grenade && !reloading)
            {
                if (Time.time > nextFireTime && !reloading && canShot && !controller.isClimbing) //Allow fire statement
                {
                    if (currentItem.weaponAmmoCount > 0)
                    {
                        currentItem.weaponAmmoCount -= 1;
                        
                        PlayFX();
                        
                        //Getting random damage from minimum and maximum damage.
                        calculatedDamage = Random.Range(damageMin, damageMax);

                        if (weaponType != WeaponType.Shotgun)
                        {
                            ProjectilesManager();
                        }
                        else
                        {
                            ProjectilesManagerShotgun(10);
                        }

                        //Playing reticle animation
                        ReticleAnimation.shot = true;

                        controller.recoil = new Vector2(Random.Range(recoil.x, recoil.x * 2), Random.Range(recoil.y, recoil.y * 2));

                        //Calculating when next fire call allowed
                        nextFireTime = Time.time + fireRate;
                    }
                    else
                    {
                        if (!reloading && autoReload)
                        {
                            if (weaponType != WeaponType.Shotgun)
                            {
                                ReloadBegin();
                            }
                        }
                        else
                        {
                            audioSource.PlayOneShot(emptySFX);
                        }

                        nextFireTime = Time.time + fireRate;
                    }
                }
            }
            else if (weaponType == WeaponType.Melee)
            {
                if (Time.time > nextFireTime) //Allow fire statement
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(shotSFX);
                    animator.Play("Shot");
                    Invoke("MeleeHit", meleeHitTime);
                    nextFireTime = Time.time + meleeAttackRate;
                }

            }
            else if(weaponType == WeaponType.Grenade && !isThrowingGrenade)
            {
                animator.SetTrigger("Throw");
                isThrowingGrenade = true;
            }
        }

        public void ThrowGrenade()
        {
            //Instantiating grenade (We are not pooling that ones because we instantiate single object which should be destroyed)
            var grenade = Instantiate(weaponSetting.grenadePrefab, transform.position, transform.rotation);
            //Applying force to grenade
            grenade.GetComponent<Rigidbody>().AddForce(grenade.transform.forward * weaponSetting.throwForce, ForceMode.Force);
            isThrowingGrenade = false;

            inventory.UseGrenade();
        }

        public void MeleeHit()
        {
            RaycastHit hit;

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, meleeAttackDistance))
            {
                audioSource.PlayOneShot(meleeHitFX);

                //Decrease health of object by calculatedDamage
                if (hit.collider.GetComponent<ObjectHealth>())
                    hit.collider.GetComponent<ObjectHealth>().health -= meleeDamagePoints;
                
                if (hit.collider.GetComponent<ZombieNPC>())
                    hit.collider.GetComponent<ZombieNPC>().ApplyHit(meleeDamagePoints);

                if (hit.collider.GetComponent<NPC>())
                    hit.collider.GetComponent<NPC>().GetHit(meleeDamagePoints, this.transform);


             //   HitParticlesFXManager(hit);

                if (hit.collider.tag =="HeadShot")
                {
                    Debug.Log("HEADSHOT!!!!");
                    hit.collider.GetComponent<HeadShot>().InstaKill();
                    hit.collider.GetComponent<HeadShot>().enabled = false;
                //    GM.headShotCount++;
                }

                if (hit.rigidbody)
                {
                    //Add force to the rigidbody for bullet impact effect
                    hit.rigidbody.AddForceAtPosition(meleeRigidbodyHitForce * cam.transform.forward, hit.point);
                }

                if (hit.collider.tag == "Target")
                {
                    hit.rigidbody.isKinematic = false;
                    hit.rigidbody.AddForceAtPosition(meleeRigidbodyHitForce * cam.transform.forward, hit.point);
                }
            }
        }

        public void FireRocketLauncher()
        {
            if (Time.time > nextFireTime && !reloading && canShot && !controller.isClimbing) //Allow fire statement
            {
                if (currentItem.weaponAmmoCount > 0)
                {
                    var obj = Instantiate(weaponSetting.projectile);
                    obj.GetComponent<Rocket>().weapon = this;

                    obj.transform.position = Camera.main.transform.position;
                    obj.transform.rotation = Camera.main.transform.rotation;

                    obj.GetComponent<Rocket>().BeginMovement();

                    currentItem.weaponAmmoCount -= 1;

                    MissileObject.SetActive(false);

                    PlayFX();
                    
                    nextFireTime = Time.time + fireRate;
                }
                else
                {
                    if (!reloading && autoReload)
                        ReloadBegin();
                    else
                        audioSource.PlayOneShot(emptySFX);

                    nextFireTime = Time.time + fireRate;
                }
            }
        }

        public void ShowMissile()
        {
            MissileObject.SetActive(true);
        }
        

        public void ApplyHit(RaycastHit hit)
        {
            //Play ricochet sfx
            RicochetSFX();
            //Set tag and transform of hit to HitParticlesFXManager
            HitParticlesFXManager(hit);

            //Decrease health of object by calculatedDamage
            if (hit.collider.GetComponent<ObjectHealth>())
                hit.collider.GetComponent<ObjectHealth>().health -= calculatedDamage;

            if (!hit.rigidbody)
            {
                //Set hit position to decal manager
                DecalManager(hit, false);
            }

            if (hit.rigidbody)
            {
                //Add force to the rigidbody for bullet impact effect
                hit.rigidbody.AddForceAtPosition(rigidbodyHitForce * damageMin * cam.transform.forward, hit.point);
            }

            if (hit.collider.tag == "Target")
            {
                hit.rigidbody.isKinematic = false;
                hit.rigidbody.AddForceAtPosition(rigidbodyHitForce * damageMin * cam.transform.forward, hit.point);
            }

            if (hit.collider.GetComponent<NPC>()) {
                if (hit.collider is BoxCollider)
                {
                    print("Hit head");
                    hit.collider.GetComponent<NPC>().GetHit(100, transform);
                }
                else if (hit.collider is CapsuleCollider)
                {
                    print("Hit body");
                    hit.collider.GetComponent<NPC>().GetHit(Random.Range(damageMin, damageMax) * 5, transform);
                }
            }
            
            if (hit.collider.GetComponent<ZombieNPC>())
                hit.collider.GetComponent<ZombieNPC>().ApplyHit(Random.Range(damageMin, damageMax));
        }

        public void ReloadBegin()
        {
            audioSource.pitch = 1;

            if (CalculateTotalAmmo() > 0)
            {
                if (weaponType == WeaponType.RocketLauncher)
                {
                    Invoke("ShowMissile", timeToShowMissileObject);
                }

                setAim = false;
                reloading = true;
                canShot = false;
                animator.SetBool("Aim", false);
                animator.Play("Reload");

                audioSource.PlayOneShot(reloadingSFX);

                Invoke("ReloadEnd", reloadAnimationDuration);
            }
            else
                return;
        }

        void ReloadEnd()
        {
            var ammoItems = GetAmmoItems();
            
            var neededAmmo = maxAmmo - currentItem.weaponAmmoCount;

            var emptyObjects = new List<ItemHandler>();

            if (ammoItems != null)
            {
                for (int i = 0; i < ammoItems.Count; i++)
                {
                    if (ammoItems[i].item.stackSize > 0)
                    {
                        if (ammoItems[i].item.stackSize >= neededAmmo)
                        {
                            ammoItems[i].item.stackSize -= neededAmmo;
                            currentItem.weaponAmmoCount += neededAmmo;

                            if (ammoItems[i].item.stackSize <= 0)
                                emptyObjects.Add(ammoItems[i]);

                            break;
                        }
                        else if (ammoItems[i].item.stackSize < neededAmmo)
                        {
                            neededAmmo -= ammoItems[i].item.stackSize;
                            currentItem.weaponAmmoCount += ammoItems[i].item.stackSize;
                            ammoItems[i].item.stackSize = 0;
                            emptyObjects.Add(ammoItems[i]);
                        }
                    }
                }
            }
            
            foreach(var item in emptyObjects)
            {
                inventory.RemoveItem(item);
            }

            reloading = false;
            canShot = true;
        }

        #region Decal, projectiles, shot FX, hitFX managers

        public void HitParticlesFXManager(RaycastHit hit)
        {
            if (hit.collider.tag == "Wood")
            {
                hitFXManager.objWoodHitFX.Stop();
                hitFXManager.objWoodHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objWoodHitFX.transform.LookAt(cam.transform.position);
                hitFXManager.objWoodHitFX.Play(true);
            }
            else if (hit.collider.tag == "Concrete")
            {
                hitFXManager.objConcreteHitFX.Stop();
                hitFXManager.objConcreteHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objConcreteHitFX.transform.LookAt(cam.transform.position);
                hitFXManager.objConcreteHitFX.Play(true);
            }
            else if (hit.collider.tag == "Dirt")
            {
                hitFXManager.objDirtHitFX.Stop();
                hitFXManager.objDirtHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objDirtHitFX.transform.LookAt(cam.transform.position);
                hitFXManager.objDirtHitFX.Play(true);
            }
            else if (hit.collider.tag == "Metal")
            {
                hitFXManager.objMetalHitFX.Stop();
                hitFXManager.objMetalHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objMetalHitFX.transform.LookAt(cam.transform.position);
                hitFXManager.objMetalHitFX.Play(true);
            }
            else if (hit.collider.tag == "Flesh")
            {
                hitFXManager.objBloodHitFX.Stop();
                hitFXManager.objBloodHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objBloodHitFX.transform.LookAt(cam.transform.position);
                hitFXManager.objBloodHitFX.Play(true);
            }
            else
            {
                hitFXManager.objConcreteHitFX.Stop();
                hitFXManager.objConcreteHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objConcreteHitFX.transform.LookAt(cam.transform.position);
                hitFXManager.objConcreteHitFX.Play(true);
            }

        }

        public void DecalManager(RaycastHit hit, bool applyParent)
        {
            if (hit.collider.CompareTag("Concrete"))
            {
                hitFXManager.concreteDecal_pool[decalIndex_concrete].SetActive(true);
                var decalPostion = hit.point + hit.normal * 0.025f;
                hitFXManager.concreteDecal_pool[decalIndex_concrete].transform.position = decalPostion;
                hitFXManager.concreteDecal_pool[decalIndex_concrete].transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                if (applyParent)
                    decals[decalIndex_concrete].transform.parent = hit.transform;

                decalIndex_concrete++;

                if (decalIndex_concrete == hitFXManager.decalsPoolSizeForEachType)
                {
                    decalIndex_concrete = 0;
                }
            }
            else if (hit.collider.CompareTag("Wood"))
            {
                hitFXManager.woodDecal_pool[decalIndex_wood].SetActive(true);
                var decalPostion = hit.point + hit.normal * 0.025f;
                hitFXManager.woodDecal_pool[decalIndex_wood].transform.position = decalPostion;
                hitFXManager.woodDecal_pool[decalIndex_wood].transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                if (applyParent)
                    decals[decalIndex_wood].transform.parent = hit.transform;

                decalIndex_wood++;

                if (decalIndex_wood == hitFXManager.decalsPoolSizeForEachType)
                {
                    decalIndex_wood = 0;
                }
            }
            else if (hit.collider.CompareTag("Dirt"))
            {
                hitFXManager.dirtDecal_pool[decalIndex_dirt].SetActive(true); var decalPostion = hit.point + hit.normal * 0.025f;
                hitFXManager.dirtDecal_pool[decalIndex_dirt].transform.position = decalPostion;
                hitFXManager.dirtDecal_pool[decalIndex_dirt].transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                if (applyParent)
                    decals[decalIndex_dirt].transform.parent = hit.transform;

                decalIndex_dirt++;

                if (decalIndex_dirt == hitFXManager.decalsPoolSizeForEachType)
                {
                    decalIndex_dirt = 0;
                }
            }
            else if (hit.collider.CompareTag("Metal"))
            {
                hitFXManager.metalDecal_pool[decalIndex_metal].SetActive(true);
                var decalPostion = hit.point + hit.normal * 0.025f;
                hitFXManager.metalDecal_pool[decalIndex_metal].transform.position = decalPostion;
                hitFXManager.metalDecal_pool[decalIndex_metal].transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                if (applyParent)
                    decals[decalIndex_metal].transform.parent = hit.transform;

                decalIndex_metal++;

                if (decalIndex_metal == hitFXManager.decalsPoolSizeForEachType)
                {
                    decalIndex_metal = 0;
                }
            }
            else
            {
                hitFXManager.concreteDecal_pool[decalIndex_concrete].SetActive(true);
                var decalPostion = hit.point + hit.normal * 0.025f;
                hitFXManager.concreteDecal_pool[decalIndex_concrete].transform.position = decalPostion;
                hitFXManager.concreteDecal_pool[decalIndex_concrete].transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                if (applyParent)
                    decals[decalIndex_concrete].transform.parent = hit.transform;

                decalIndex_concrete++;

                if (decalIndex_concrete == hitFXManager.decalsPoolSizeForEachType)
                {
                    decalIndex_concrete = 0;
                }
            }
        }

        public void ProjectilesManager()
        {
            var spreadWithMovementFactor = spread + movementSpreadFactor;

            ///Make lower spread factor if player is aiming
            if (setAim && weaponType != WeaponType.Shotgun)
                spreadWithMovementFactor /= 3;

            Vector3 spreadVector = new Vector3(Random.Range(-spreadWithMovementFactor, spreadWithMovementFactor), Random.Range(-spreadWithMovementFactor, spreadWithMovementFactor), Random.Range(-spreadWithMovementFactor, spreadWithMovementFactor));
            
                projectiles[projectilesIndex].transform.position = Camera.main.transform.position;
                projectiles[projectilesIndex].transform.rotation = Camera.main.transform.rotation;
                projectiles[projectilesIndex].transform.Rotate(spreadVector);

            if (weaponType == WeaponType.SniperRiffle && setAim)
            {
                projectiles[projectilesIndex].transform.position = Camera.main.transform.position;
                projectiles[projectilesIndex].transform.rotation = Camera.main.transform.rotation;
            }

            projectiles[projectilesIndex].SetActive(true);

            projectilesIndex++;

            if (projectilesIndex == projectiles.Length)
            {
                projectilesIndex = 0;
            }
        }

        public void PlayReloadSfxShotgun()
        {
            audioSource.PlayOneShot(reloadingSFX);
        }

        public void PlayLoadSfxShotgun()
        {
            audioSource.PlayOneShot(weaponSetting.shotgunLoadingClip);
        }

        public void ProjectilesManagerShotgun(int bulletCount)
        {
            var spreadWithMovementFactor = spread * 5;
            
            for (int i = 0; i < bulletCount; i++)
            {
                Vector3 spreadVector = new Vector3(Random.Range(-spreadWithMovementFactor, spreadWithMovementFactor), Random.Range(-spreadWithMovementFactor, spreadWithMovementFactor), Random.Range(-spreadWithMovementFactor, spreadWithMovementFactor));

                projectiles[projectilesIndex].transform.position = Camera.main.transform.position;
                projectiles[projectilesIndex].transform.rotation = Camera.main.transform.rotation;
                projectiles[projectilesIndex].transform.Rotate(spreadVector);

                projectiles[projectilesIndex].SetActive(true);

                projectilesIndex++;

                if (projectilesIndex == projectiles.Length)
                {
                    projectilesIndex = 0;
                }
            }
        }

        private void PlayFX()
        {
                //animator.Play("Shot");
                if (setAim && weaponType != WeaponType.Pistol && weaponType != WeaponType.Shotgun)
                {
                    weaponHolderAnimator.Play("AimShot");
                }
                else
                {
                        var range = Random.Range(0, 2);

                        if (range == 0 || weaponType == WeaponType.Shotgun)
                            animator.Play("Shot");
                        if (range == 1 && weaponType != WeaponType.Shotgun)
                            animator.Play("Shot1");
                        if (range == 2 && weaponType != WeaponType.Shotgun)
                            animator.Play("Shot2");
                }
            

            temp_MuzzleFlashParticlesFX.time = 0;
            temp_MuzzleFlashParticlesFX.Play();

            audioSource.Stop();
            if(weaponType == WeaponType.SMG) 
            audioSource.pitch = Random.Range(0.9f, 1.4f);
            audioSource.PlayOneShot(shotSFX);

            if (shells != null)
            {
                if (weaponType != WeaponType.SniperRiffle)
                {
                    shells[shellIndex].GetComponent<Rigidbody>().velocity = Vector3.zero;
                    shells[shellIndex].SetActive(true);
                    shells[shellIndex].transform.localPosition = shellTransform.transform.position;
                    shells[shellIndex].transform.localRotation = shellTransform.transform.rotation;
                    shells[shellIndex].GetComponent<Rigidbody>().AddForce(shellTransform.transform.forward * weaponSetting.shellEjectingForce, ForceMode.Force);
                    shellIndex++;
                }

                if (shellIndex == shells.Length)
                {
                    shellIndex = 0;
                }
            }
        }

        public void RicochetSFX()
        {
            //ricochetSource.Stop();
            //ricochetSource.PlayOneShot(ricochetSounds[Random.Range(0, ricochetSounds.Length)]);
        }

        void ShotgunReload()
        {
            if (CalculateTotalAmmo() == 0)
                return;

            if (animator.GetBool("Reloading") == false)
            {
                reloading = true;
                animator.Play("ReloadBegin");
                animator.SetBool("Reloading", true);
            }
        }

        void SetReloadingStateFalse() { reloading = false; }

        void AddShotgunAmmo()
        {
            var ammoItems = GetAmmoItems();

            var neededAmmo = 1;

            var emptyObjects = new List<ItemHandler>();

            if (ammoItems != null)
            {
                for (int i = 0; i < ammoItems.Count; i++)
                {
                    if (ammoItems[i].item.stackSize > 0)
                    {
                        if (ammoItems[i].item.stackSize >= neededAmmo)
                        {
                            ammoItems[i].item.stackSize -= neededAmmo;
                            currentItem.weaponAmmoCount += neededAmmo;

                            if (ammoItems[i].item.stackSize <= 0)
                                emptyObjects.Add(ammoItems[i]);

                            break;
                        }
                        else if (ammoItems[i].item.stackSize < neededAmmo)
                        {
                            neededAmmo -= ammoItems[i].item.stackSize;
                            currentItem.weaponAmmoCount += ammoItems[i].item.stackSize;
                            ammoItems[i].item.stackSize = 0;
                            emptyObjects.Add(ammoItems[i]);
                        }
                    }
                }
            }

            foreach (var item in emptyObjects)
            {
                inventory.RemoveItem(item);
            }
            
        }
        
        #endregion

        #region Pool methods

        public void ShellsPool()
        {
            shells = new GameObject[shellPoolSize];
            var shellsParentObject = new GameObject(weaponName + "_shellsPool");

            shellsParentObject.transform.SetParent(GameObject.Find("GamePrefab").transform);

            for (int i = 0; i < shellPoolSize; i++)
            {
                shells[i] = Instantiate(shell);
                shells[i].SetActive(false);
                shells[i].transform.parent = shellsParentObject.transform;
            }
        }

        public void BalisticProjectilesPool()
        {
            projectiles = new GameObject[projectilePoolSize];

            var projectileSettingObject = Instantiate(projectile);
            projectileSettingObject.SetActive(false);
            projectileSettingObject.GetComponentInChildren<BalisticProjectile>().weapon = this;
            projectileSettingObject.GetComponentInChildren<BalisticProjectile>().initialVelocity = bulletInitialVelocity;
            projectileSettingObject.GetComponentInChildren<BalisticProjectile>().airResistance = airResistanceForce;

            var projectilesParentObject = new GameObject(weaponName + "_projectilesPool" + " " + weaponName);
            projectilesParentObject.transform.SetParent(GameObject.Find("GamePrefab").transform);

            for (int i = 0; i < projectilePoolSize; i++)
            {
                projectiles[i] = Instantiate(projectileSettingObject);
                projectiles[i].SetActive(false);
                projectiles[i].transform.parent = projectilesParentObject.transform;
            }
        }

        #endregion

        #region Utility methods

        public void SetAim()
        {
            if (!reloading && !InventoryManager.showInventory)
            {
                animator.SetBool("Aim", setAim);
            }
            else
            {
                setAim = false;
            }
            
            if (setAim)
            {
                    controller.sensitivity.x = normalSensX / 2;
                    controller.sensitivity.y = normalSensY / 2;
            }
            else
            {
                controller.sensitivity.x = normalSensX;
                controller.sensitivity.y = normalSensY;
            }
        }
        
        
        private void FireModeSwitch()
        {
            if(Input.GetKeyDown(input.FiremodeAuto))
            {
                fireMode = FireMode.automatic;
            }
            if(Input.GetKeyDown(input.FiremodeSingle))
            {
                fireMode = FireMode.single;
            }
        }

        private void UpdateAmmoText()
        {
            if (!ammoText.enabled)
                ammoText.enabled = true;

            char mode = new char();

            if(fireMode == FireMode.single)
            {
                mode = '1';
            }
            else
            {
                mode = 'A';
            }

            weaponNameText.text = weaponName;
            if(weaponType != WeaponType.Grenade)
            ammoText.text = string.Format("{0}|{1}\n Mode:{2}", currentItem.weaponAmmoCount, CalculateTotalAmmo(), mode);
        }

        public int CalculateTotalAmmo()
        {
            int totalAmmo = new int();
            
            foreach (var UIItem in inventory.ammoItems)
            {
                if (UIItem.item.id == ammoItemID)
                {
                    totalAmmo += UIItem.item.stackSize;
                }
            }

            return totalAmmo;
        }

        public List<ItemHandler> GetAmmoItems()
        {
            var items = new List<ItemHandler>();
            foreach (var UIItem in inventory.ammoItems)
            {
                if (UIItem.item.id == ammoItemID)
                    items.Add(UIItem);
            }
            
            return items;
        }

        #endregion
        
        private void OnDisable()
        {
            CancelInvoke();

            try
            {


                reticle.SetActive(true);

                if (animator != null)
                {
                    if (animator.isActiveAndEnabled)
                    {
                        animator.Play("Default position");
                        animator.WriteDefaultValues();
                        animator.Rebind();
                    }
                }

                canShot = true;
                reloading = false;

                if (weaponNameText)
                    weaponNameText.text = "";
                if (ammoText)
                    ammoText.text = "";
            }
            catch
            {

            }
        }
    }
}
