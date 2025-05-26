using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class ParticlesEntity : Entity
    {
        public bool IsEmitting { get; set; } = true;

        protected List<MParticle> particles = new();
        
        private float emissionRate;
        private float timeAccumulator = 0f;
        private Func<MParticle> particleFactory;

        private ISpawnShape defaultSpawnShape = new MPointShape();
        private IParticleRenderer defaultRenderer = new StaticParticleRenderer();

        private ISpawnShape currentSpawnShape;
        private IParticleRenderer currentRenderer;

        private List<IParticleModifier> modifiers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emissionRate">Particles per second</param>
        /// <param name="particleFactory"></param>
        public ParticlesEntity(float emissionRate, Func<MParticle> particleFactory)
        {
            this.emissionRate = emissionRate;
            this.particleFactory = particleFactory;

            currentSpawnShape = defaultSpawnShape;
            currentRenderer = defaultRenderer;

            modifiers = new List<IParticleModifier>();
        }

        public override void Update()
        {
            base.Update();

            if (IsEmitting)
            {
                timeAccumulator += Engine.GameDeltaTime;
                float timePerParticle = 1f / emissionRate;

                while (timeAccumulator >= timePerParticle)
                {
                    var particle = particleFactory();
                    
                    particle.Position = currentSpawnShape.GetSpawnPosition(GlobalPosition);
                    
                    particles.Add(particle);
                    
                    timeAccumulator -= timePerParticle;
                }
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var p = particles[i];

                p.Update(Engine.GameDeltaTime);

                foreach (var modifier in modifiers)
                {
                    modifier.Update(p, Engine.GameDeltaTime);
                }

                if (p.IsAlive == false)
                {
                    particles.RemoveAt(i);
                }
            }
        }

        public override void Render()
        {
            base.Render();

            foreach (var p in particles)
            {
                currentRenderer.Draw(p);
            }
        }

        public void SetSpawnShape(ISpawnShape spawnShape)
        {
            if(spawnShape == null)
            {
                currentSpawnShape = defaultSpawnShape;
                return;
            }

            currentSpawnShape = spawnShape;
        }

        public void SetRenderer(IParticleRenderer renderer)
        {
            if (renderer == null)
            {
                currentRenderer = defaultRenderer;
                return;
            }

            currentRenderer = renderer;
        }

        public void AddModifier(IParticleModifier modifier)
        {
            modifiers.Add(modifier);
        }
    }
}
