using System;
using ES.EvPointer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ES
{
    public abstract class PointerSetParticle_Abstract : IPointerNone
    {
        [LabelText("直接引用ParticleSystem")] public ParticleSystem ParticleSystem;

        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (ParticleSystem != null)
                PickTruely(ParticleSystem);
            return -1;
        }

        public abstract void PickTruely(ParticleSystem particleSystem);
    }

    // 1. Duration
    [Serializable]
    [TypeRegistryItem("Particle_To_SetDuration", "ParticleSystem")]
    public class PointerSetParticle_SetDuration : PointerSetParticle_Abstract
    {
        [LabelText("持续时长")] public float value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.duration = value;
        }
    }

    // 2. Looping
    [Serializable]
    [TypeRegistryItem("Particle_To_SetLooping", "ParticleSystem")]
    public class PointerSetParticle_SetLooping : PointerSetParticle_Abstract
    {
        [LabelText("循环播放")] public bool value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.loop = value;
        }
    }

    // 3. Prewarm
    [Serializable]
    [TypeRegistryItem("Particle_To_SetPrewarm", "ParticleSystem")]
    public class PointerSetParticle_SetPrewarm : PointerSetParticle_Abstract
    {
        [LabelText("预热开启")] public bool value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.prewarm = value;
        }
    }

    // 4. Start Delay
    [Serializable]
    [TypeRegistryItem("Particle_To_SetStartDelay", "ParticleSystem")]
    public class PointerSetParticle_SetStartDelay : PointerSetParticle_Abstract
    {
        [LabelText("开始延迟")] public float value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.startDelay = value;
        }
    }

    // 5. Start Lifetime
    [Serializable]
    [TypeRegistryItem("Particle_To_SetStartLifetime", "ParticleSystem")]
    public class PointerSetParticle_SetStartLifetime : PointerSetParticle_Abstract
    {
        [LabelText("起始存活时间")] public float value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.startLifetime = value;
        }
    }

    // 6. Start Speed
    [Serializable]
    [TypeRegistryItem("Particle_To_SetStartSpeed", "ParticleSystem")]
    public class PointerSetParticle_SetStartSpeed : PointerSetParticle_Abstract
    {
        [LabelText("起始速度")] public float value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.startSpeed = value;
        }
    }

    // 7. 3D Start Size
    [Serializable]
    [TypeRegistryItem("Particle_To_SetStartSize3D", "ParticleSystem")]
    public class PointerSetParticle_SetStartSize3D : PointerSetParticle_Abstract
    {
        [LabelText("启用3D起始大小")] public bool value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.startSize3D = value;
        }
    }

    //    Start Size
    [Serializable]
    [TypeRegistryItem("Particle_To_SetStartSize", "ParticleSystem")]
    public class PointerSetParticle_SetStartSize : PointerSetParticle_Abstract
    {
        [LabelText("起始大小")] public float value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.startSize = value;
        }
    }

    // 8. 3D Start Rotation
    [Serializable]
    [TypeRegistryItem("Particle_To_SetStartRotation3D", "ParticleSystem")]
    public class PointerSetParticle_SetStartRotation3D : PointerSetParticle_Abstract
    {
        [LabelText("启用3D起始旋转")] public bool value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.startRotation3D = value;
        }
    }

    //    Start Rotation
    [Serializable]
    [TypeRegistryItem("Particle_To_SetStartRotation", "ParticleSystem")]
    public class PointerSetParticle_SetStartRotation : PointerSetParticle_Abstract
    {
        [LabelText("起始旋转(角度)")] public float value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.startRotation = value * Mathf.Deg2Rad;
        }
    }

    // 9. Flip Rotation
    [Serializable]
    [TypeRegistryItem("Particle_To_SetFlipRotation", "ParticleSystem")]
    public class PointerSetParticle_SetFlipRotation : PointerSetParticle_Abstract
    {
        [LabelText("翻转旋转(角度)")] public float value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.flipRotation = value * Mathf.Deg2Rad;
        }
    }

    // 10. Start Color
    [Serializable]
    [TypeRegistryItem("Particle_To_SetStartColor", "ParticleSystem")]
    public class PointerSetParticle_SetStartColor_GameObject : PointerSetParticle_Abstract
    {
        [LabelText("起始颜色")] public Color value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.startColor = value;
        }
    }

    // 11. Gravity Modifier
    [Serializable]
    [TypeRegistryItem("Particle_To_SetGravityModifier", "ParticleSystem")]
    public class PointerSetParticle_SetGravityModifier : PointerSetParticle_Abstract
    {
        [LabelText("重力系数")] public float value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.gravityModifier = value;
        }
    }

    // 12. Simulation Space
    [Serializable]
    [TypeRegistryItem("Particle_To_SetSimulationSpace", "ParticleSystem")]
    public class PointerSetParticle_SetSimulationSpace : PointerSetParticle_Abstract
    {
        [LabelText("模拟空间")] public ParticleSystemSimulationSpace value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.simulationSpace = value;
        }
    }

    // 13. Simulation Speed
    [Serializable]
    [TypeRegistryItem("Particle_To_SetSimulationSpeed", "ParticleSystem")]
    public class PointerSetParticle_SetSimulationSpeed : PointerSetParticle_Abstract
    {
        [LabelText("模拟速率")] public float value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.simulationSpeed = value;
        }
    }

    // 14. Use Unscaled Time
    [Serializable]
    [TypeRegistryItem("Particle_To_SetUnscaledTime", "ParticleSystem")]
    public class PointerSetParticle_SetUnscaledTime : PointerSetParticle_Abstract
    {
        [LabelText("使用非缩放时间")] public bool value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.useUnscaledTime = value;
        }
    }

    // 15. Scaling Mode
    [Serializable]
    [TypeRegistryItem("Particle_To_SetScalingMode", "ParticleSystem")]
    public class PointerSetParticle_SetScalingMode : PointerSetParticle_Abstract
    {
        [LabelText("缩放模式")] public ParticleSystemScalingMode value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.scalingMode = value;
        }
    }

    // 16. Play On Awake
    [Serializable]
    [TypeRegistryItem("Particle_To_SetPlayOnAwake", "ParticleSystem")]
    public class PointerSetParticle_SetPlayOnAwake : PointerSetParticle_Abstract
    {
        [LabelText("启动即播放")] public bool value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.playOnAwake = value;
        }
    }

    // 17. Max Particles
    [Serializable]
    [TypeRegistryItem("Particle_To_SetMaxParticles", "ParticleSystem")]
    public class PointerSetParticle_SetMaxParticles : PointerSetParticle_Abstract
    {
        [LabelText("最大粒子数")] public int value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.maxParticles = value;
        }
    }

    // 18. Auto Random Seed
    [Serializable]
    [TypeRegistryItem("Particle_To_SetAutoRandomSeed", "ParticleSystem")]
    public class PointerSetParticle_SetAutoRandomSeed : PointerSetParticle_Abstract
    {
        [LabelText("自动随机种子")] public bool value;

        [ShowIf("@!this.value")] [LabelText("随机种子值")]
        public uint seedValue;

        public override void PickTruely(ParticleSystem ps)
        {
            ps.useAutoRandomSeed = value;
            if(!value)ps.randomSeed = seedValue;
        }
    }

    // 19. Stop Action
    [Serializable]
    [TypeRegistryItem("Particle_To_SetStopAction", "ParticleSystem")]
    public class PointerSetParticle_SetStopAction : PointerSetParticle_Abstract
    {
        [LabelText("停止动作")] public ParticleSystemStopAction value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.stopAction = value;
        }
    }

    // 20. Culling Mode
    [Serializable]
    [TypeRegistryItem("Particle_To_SetCullingMode", "ParticleSystem")]
    public class PointerSetParticle_SetCullingMode : PointerSetParticle_Abstract
    {
        [LabelText("裁剪模式")] public ParticleSystemCullingMode value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.cullingMode = value;
        }
    }

    // 21. Ring Buffer Mode
    [Serializable]
    [TypeRegistryItem("Particle_To_SetRingBufferMode", "ParticleSystem")]
    public class PointerSetParticle_SetRingBufferMode : PointerSetParticle_Abstract
    {
        [LabelText("环形缓冲模式")] public ParticleSystemRingBufferMode value;

        public override void PickTruely(ParticleSystem ps)
        {
            var main = ps.main;
            main.ringBufferMode = value;
        }
    }
}