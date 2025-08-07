using System;
using System.Collections;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Interfaces.Common;
using _1.Scripts.Interfaces.NPC;
using _1.Scripts.Manager.Core;
using _1.Scripts.Manager.Subs;
using _1.Scripts.Static;
using _1.Scripts.Util;
using BehaviorDesigner.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _1.Scripts.Entity.Scripts.Npc.Unit
{
	/// <summary>
	/// 드론관련 애니메이션 이벤트가 너무 많아 분리. 공통 및 Shebot 애니메이션 이벤트는 AIControlWithAnimationEvent.cs에 존재
	/// </summary>
	public class Unit_DroneBot : MonoBehaviour
	{
		public Transform Gun_EndR;
		public Transform Gun_EndL;

		public ParticleSystem[] p_jet;
		private bool restartRes = true;
		private Transform pos_side;

		public ParticleSystem p_dead, p_smoke, p_fireL, p_fireSmokeL, p_fireR, p_fireSmokeR; //Particle effect  
		private AudioSource m_AudioSource;

		private Coroutine gotDamagedCoroutine;
		private float gotDamagedParticleDuration = 0.5f;

		private BehaviorTree behaviorTree;
		private BaseNpcStatController statController;
		[SerializeField] private float spreadAmount = 0.5f;

		private void Awake()
		{
			m_AudioSource = GetComponent<AudioSource>();
			behaviorTree = GetComponent<BehaviorTree>();
			statController = GetComponent<BaseNpcStatController>();
		}

		private void OnDisable()
		{
			restartRes = false;
		}

		void f_afterFire()
		{
			p_fireSmokeL.Play();
			p_fireSmokeR.Play();
		}

		void f_start()
		{
			if (!restartRes)
			{
				restartRes = true;
				m_AudioSource.loop = true;
				m_AudioSource.Play();

				for (int i = 0; i < p_jet.Length; i++)
				{
					p_jet[i].Play();
				}
			}
		}

		void f_dead() //dead
		{
			for (int i = 0; i < p_jet.Length; i++)
			{
				p_jet[i].Stop();
			}

			p_dead.Play();
			p_smoke.Play();
			CoreManager.Instance.soundManager.PlaySFX(SfxType.Drone, transform.position, index: 2);
			m_AudioSource.Stop();
			m_AudioSource.loop = false;
			restartRes = false;
		}

		void f_fire(int side) //shot 
		{
			var targetTransform = behaviorTree.GetVariable(BehaviorNames.TargetTransform) as SharedTransform;
			var targetPos = behaviorTree.GetVariable(BehaviorNames.TargetPos) as SharedVector3;
			bool isAlly = statController.RuntimeStatData.IsAlly;
			int damage = statController.RuntimeStatData.BaseDamage;

			if (targetTransform == null || targetTransform.Value == null) return;

			if (side == 1)
			{
				p_fireR.Play();
				pos_side = Gun_EndR.transform;
			}
			else
			{
				p_fireL.Play();
				pos_side = Gun_EndL.transform;
			}

			Vector3 directionToTarget = (targetPos.Value - pos_side.position).normalized;

			directionToTarget.x += Random.Range(-spreadAmount, spreadAmount);
			directionToTarget.y += Random.Range(-spreadAmount, spreadAmount);
			directionToTarget.z += Random.Range(-spreadAmount, spreadAmount);
			directionToTarget.Normalize();

			NpcUtil.FireToTarget(pos_side.position, directionToTarget, isAlly, damage);
			CoreManager.Instance.soundManager.PlaySFX(SfxType.Drone, transform.position, -1, 0);
		}
	}
}