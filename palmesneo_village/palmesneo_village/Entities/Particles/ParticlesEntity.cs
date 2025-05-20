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

        private ISpawnShape spawnShape = new MPointShape();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emissionRate">Particles per second</param>
        /// <param name="particleFactory"></param>
        public ParticlesEntity(float emissionRate, Func<MParticle> particleFactory)
        {
            this.emissionRate = emissionRate;
            this.particleFactory = particleFactory;
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
                    
                    particle.Position = spawnShape.GetSpawnPosition(GlobalPosition);
                    
                    particles.Add(particle);
                    
                    timeAccumulator -= timePerParticle;
                }
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var p = particles[i];

                p.Update(Engine.GameDeltaTime);

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
                p.Draw(RenderManager.SpriteBatch);
            }
        }

        public void SetSpawnShape(ISpawnShape spawnShape)
        {
            this.spawnShape = spawnShape;
        }
    }
}
