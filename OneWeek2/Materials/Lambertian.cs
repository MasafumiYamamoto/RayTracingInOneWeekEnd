﻿using System.Numerics;

namespace OneWeek2.Materials
{
    public class Lambertian : Material
    {
        public Vector3 Albedo { get; }

        public Lambertian(Vector3 albedo)
        {
            Albedo = albedo;
        }
        
        public override bool Scatter(in Ray ray, in HitRecord hitRecord, out Vector3 attenuation, out Ray scattered)
        {
            var scatterDirection = hitRecord.Normal + MathHelper.RandomUnitVector();
            scattered = new Ray(hitRecord.P, scatterDirection);
            attenuation = Albedo;
            return true;
        }
    }
}