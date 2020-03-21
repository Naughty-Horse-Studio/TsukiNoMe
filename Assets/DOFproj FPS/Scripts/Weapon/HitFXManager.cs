/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---


using UnityEngine;

namespace DOFprojFPS
{
    public class HitFXManager : MonoBehaviour
    {
        [Header("Hit Particles FX")]
        public ParticleSystem concreteHitFX;
        public ParticleSystem woodHitFX;
        public ParticleSystem dirtHitFX;
        public ParticleSystem metalHitFX;
        public ParticleSystem bloodHitFX;

        [HideInInspector]
        public ParticleSystem objConcreteHitFX;
        [HideInInspector]
        public ParticleSystem objWoodHitFX;
        [HideInInspector]
        public ParticleSystem objDirtHitFX;
        [HideInInspector]
        public ParticleSystem objMetalHitFX;
        [HideInInspector]
        public ParticleSystem objBloodHitFX;

        [Header("Melee sounds")]
        public AudioClip impactSound;

        [Header("Ricochet sounds")]
        public AudioClip[] ricochetSounds;

        [Header("Decals")]

        [Range(1, 200)]
        [Tooltip("Pool size for each type of decals. For example, if pool size is 10, concrete, wood, dirt, metal decals will have 10 their instances after start")]
        public int decalsPoolSizeForEachType;

        public GameObject concreteDecal;
        public GameObject woodDecal;
        public GameObject dirtDecal;
        public GameObject metalDecal;
        
        [HideInInspector]
        public GameObject[] concreteDecal_pool;
        [HideInInspector]
        public GameObject[] woodDecal_pool;
        [HideInInspector]
        public GameObject[] dirtDecal_pool;
        [HideInInspector]
        public GameObject[] metalDecal_pool;

        public bool usedByNPC = true;

        void Start()
        {
            DecalsPool();

            objConcreteHitFX = Instantiate(concreteHitFX);
            if (!usedByNPC)
                objConcreteHitFX.transform.SetParent(GameObject.Find("GamePrefab").transform);

            objWoodHitFX = Instantiate(woodHitFX);
            if (!usedByNPC)
                objWoodHitFX.transform.SetParent(GameObject.Find("GamePrefab").transform);

            objDirtHitFX = Instantiate(dirtHitFX);
            if (!usedByNPC)
                objDirtHitFX.transform.SetParent(GameObject.Find("GamePrefab").transform);

            objMetalHitFX = Instantiate(metalHitFX);
            if (!usedByNPC)
                objMetalHitFX.transform.SetParent(GameObject.Find("GamePrefab").transform);

            objBloodHitFX = Instantiate(bloodHitFX);
            if (!usedByNPC)
                objBloodHitFX.transform.SetParent(GameObject.Find("GamePrefab").transform);

        }

        public void DecalsPool()
        {
            concreteDecal_pool = new GameObject[decalsPoolSizeForEachType];
            var decalsParentObject_concrete = new GameObject("decalsPool_concrete");

            if (!usedByNPC)
            {
                decalsParentObject_concrete.transform.SetParent(GameObject.Find("GamePrefab").transform);
            }

            for (int i = 0; i < decalsPoolSizeForEachType; i++)
            {
                concreteDecal_pool[i] = Instantiate(concreteDecal);
                concreteDecal_pool[i].SetActive(false);
                concreteDecal_pool[i].transform.parent = decalsParentObject_concrete.transform;
            }

            woodDecal_pool = new GameObject[decalsPoolSizeForEachType];
            var decalsParentObject_wood = new GameObject("decalsPool_wood");

            if(!usedByNPC)
            decalsParentObject_wood.transform.SetParent(GameObject.Find("GamePrefab").transform);

            for (int i = 0; i < decalsPoolSizeForEachType; i++)
            {
                woodDecal_pool[i] = Instantiate(woodDecal);
                woodDecal_pool[i].SetActive(false);
                woodDecal_pool[i].transform.parent = decalsParentObject_wood.transform;
            }

            dirtDecal_pool = new GameObject[decalsPoolSizeForEachType];
            var decalsParentObject_dirt = new GameObject("decalsPool_dirt");

            if (!usedByNPC)
                decalsParentObject_dirt.transform.SetParent(GameObject.Find("GamePrefab").transform);

            for (int i = 0; i < decalsPoolSizeForEachType; i++)
            {
                dirtDecal_pool[i] = Instantiate(dirtDecal);
                dirtDecal_pool[i].SetActive(false);
                dirtDecal_pool[i].transform.parent = decalsParentObject_dirt.transform;
            }

            metalDecal_pool = new GameObject[decalsPoolSizeForEachType];
            
            var decalsParentObject_metal = new GameObject("decalsPool_metal");

            if (!usedByNPC)
                decalsParentObject_metal.transform.SetParent(GameObject.Find("GamePrefab").transform);

            for (int i = 0; i < decalsPoolSizeForEachType; i++)
            {
                metalDecal_pool[i] = Instantiate(metalDecal);
                metalDecal_pool[i].SetActive(false);
                metalDecal_pool[i].transform.parent = decalsParentObject_metal.transform;
            }

        }
    }
}
