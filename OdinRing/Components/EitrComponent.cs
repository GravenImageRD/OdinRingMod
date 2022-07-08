using Jotunn.Managers;
using OdinRing.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OdinRing.Components
{
    public class EitrComponent : MonoBehaviour
    {
        float startTime;
        bool seeking = true;
        Rigidbody rb;

        public Player Player { get; set; }
        public string Monster { get; set; }

        private void Awake()
        {
            startTime = Time.time;
            rb = GetComponent<Rigidbody>();
        }

        void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<Player>();
            Jotunn.Logger.LogInfo($"Collision with {other.name}");
            if (player != null)
            {
                Jotunn.Logger.LogInfo($"Collision with player {player.name}");
                if (player == Player)
                {
                    var eitrPrefab = PrefabManager.Instance.GetPrefab("vfx_eitrGained");
                    var eitr = GameObject.Instantiate(eitrPrefab, Player.gameObject.transform.position + Vector3.up, Quaternion.identity);
                    var eitrGainedComponent = eitr.AddComponent<EitrGainedComponent>();
                    eitrGainedComponent.Player = Player;
                    Player.GetOdinRingData().GiveEitr(Monster);
                    seeking = false;
                    rb.velocity = Vector3.zero;
                    startTime = Time.time + 2f;
                    GetComponentInChildren<ParticleSystem>().Stop();
                }
            }
        }

        private void FixedUpdate()
        {
            if (Player == null)
            {
                return;
            }

            if (!seeking)
            {
                if (Time.time > startTime)
                {
                    ZNetScene.instance.Destroy(gameObject);
                }
                return;
            }

            float t = Time.time - startTime;
            Vector3 vectorToTarget = Player.transform.position - transform.position + (Vector3.up * 1.1f);
            rb.velocity = vectorToTarget * t * t;
        }
    }
}
