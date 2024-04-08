using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class DeckSystemScript : MonoBehaviour
{
    [Header("Keybind Inputs")]
    public InputAction input1;
    public InputAction input2;
    public InputAction input3;

    [Header("Spell Deck")]
    public List<Spell> totalSpells = new List<Spell>();
    public List<Spell> shuffledSpells = new List<Spell>();

    [Header("Object References")]
    [SerializeField] public Camera cam;
    [SerializeField] public GameObject spellSpawnArea;

    [Header("Spell Prefab")]
    [SerializeField] public GameObject fireBallPrefab;
    [SerializeField] public GameObject lightningStrikePrefab;
    [SerializeField] public GameObject toxicCloudPrefab;
    [SerializeField] public GameObject revitilizePrefab;

    [Header("Spell Sprites")]
    [SerializeField] public Sprite fireBallSprite;
    [SerializeField] public Sprite lightningStrikeSprite;
    [SerializeField] public Sprite toxicCloudSprite;
    [SerializeField] public Sprite revitilizeSprite;

    int spellCooldownDuration = 2;
    private bool isCooldown = false;

    #region Base Spell Class
    [Serializable]
    public class Spell
    {
        // Spell Template
        public virtual void Cast(GameObject spawnPoint) { }
    }
    #endregion

    #region Unique Spell Classes
    public class FireballSpell : Spell
    {
        public GameObject fireballPrefab;
        public override void Cast(GameObject spawnPoint)
        {
            var pos = spawnPoint.transform.position + spawnPoint.transform.forward;
            var rot = spawnPoint.transform.rotation;
            Instantiate(fireballPrefab, pos, rot);
        }
    }

    public class LightningStrikeSpell : Spell
    {
        public Camera lsCam;
        public GameObject lightningStrikePrefab;
        public override void Cast(GameObject spawnPoint)
        {
            RaycastHit hit;
            if (Physics.Raycast(lsCam.transform.position, lsCam.transform.forward, out hit, 100f))
            {
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Instantiate(lightningStrikePrefab, hit.point, rotation);
            }
        }
    }

    public class ToxicCloudSpell : Spell
    {
        public Camera tcCam;
        public GameObject toxicCloudPrefab;
        public override void Cast(GameObject spawnPoint)
        {
            RaycastHit hit;
            if (Physics.Raycast(tcCam.transform.position, tcCam.transform.forward, out hit, 100f))
            {
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Instantiate(toxicCloudPrefab, hit.point, rotation);
            }
        }
    }

    public class Revitilize : Spell
    {
        PlayerController playerHP;
        public override void Cast(GameObject spawnPoint)
        {
            playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            playerHP.ps.health += 10f;
        }
    }
    #endregion

    void Start()
    {
        FireballSpell fireballSpell = new FireballSpell();
        fireballSpell.fireballPrefab = fireBallPrefab;

        LightningStrikeSpell lightningStrikeSpell = new LightningStrikeSpell();
        lightningStrikeSpell.lsCam = cam;
        lightningStrikeSpell.lightningStrikePrefab = lightningStrikePrefab;

        ToxicCloudSpell toxicCloudSpell = new ToxicCloudSpell();
        toxicCloudSpell.tcCam = cam;
        toxicCloudSpell.toxicCloudPrefab = toxicCloudPrefab;

        totalSpells.Add(fireballSpell);
        totalSpells.Add(lightningStrikeSpell);
        totalSpells.Add(toxicCloudSpell);
        totalSpells.Add(lightningStrikeSpell);
        totalSpells.Add(fireballSpell);
    }

    void Update()
    {
        if (totalSpells.Count != 0)
        {
            if (shuffledSpells.Count == 0)
            {
                ShuffleDeck(totalSpells);
            }
            else
            {
                SpellCasting();
            }
        }
    }

    public void ShuffleDeck(List<Spell> shuffling)
    {
        shuffledSpells.Clear();
        shuffledSpells.AddRange(totalSpells);

        System.Random rng = new System.Random();
        int n = shuffledSpells.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Spell temp = shuffledSpells[k];
            shuffledSpells[k] = shuffledSpells[n];
            shuffledSpells[n] = temp;
        }
    }

    public void SpellCasting()
    {
        if (!isCooldown)
        {
            if (input1.WasPressedThisFrame())
            {
                shuffledSpells[0].Cast(spellSpawnArea);
                shuffledSpells.RemoveAt(0);
                StartCoroutine(SpellCooldown());
            }
            if (input2.WasPressedThisFrame())
            {
                if (shuffledSpells.Count > 1)
                {
                    shuffledSpells[1].Cast(spellSpawnArea);
                    shuffledSpells.RemoveAt(1);
                    StartCoroutine(SpellCooldown());
                }
                else
                {
                    return;
                }
            }
            if (input3.WasPressedThisFrame())
            {
                if (shuffledSpells.Count > 2)
                {
                    shuffledSpells[2].Cast(spellSpawnArea);
                    shuffledSpells.RemoveAt(2);
                    StartCoroutine(SpellCooldown());
                }
                else
                {
                    return;
                }
            }
        }
    }

    private IEnumerator SpellCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(spellCooldownDuration);
        isCooldown = false;
    }

    #region Inputs Enabler & Disabler
    private void OnEnable()
    {
        input1.Enable();
        input2.Enable();
        input3.Enable();
    }
    private void OnDisable()
    {
        input1.Disable();
        input2.Disable();
        input3.Disable();
    }
    #endregion
}
