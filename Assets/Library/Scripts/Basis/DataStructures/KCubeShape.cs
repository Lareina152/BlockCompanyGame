using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GenericBasis;
using UnityEngine;

namespace Basis
{
    #region Shape

    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    [JsonObject(MemberSerialization.OptIn)]
    [InfoBox(@"@$value.sizeName + ""为非正值""", InfoMessageType.Warning,
        @"@$value != null && $value.requireCheckSize && GenericNumberFunc.AnyNumberBelowOrEqual($value.size, 0.0)")]
    [Serializable]
    public abstract class KCubeShape<TPointType> : ICloneable
        where TPointType : struct, IEquatable<TPointType>
    {

        protected virtual string pointName => "点";

        protected virtual string sizeName => "尺寸";

        protected virtual bool requireCheckSize => true;

        [LabelText(@"@""最小"" + pointName")]
        [OnValueChanged(nameof(OnBoundsChanged))]
        [JsonProperty]
        public TPointType minPos;

        [LabelText(@"@""最大"" + pointName")]
        [InfoBox(@"@""最大"" + pointName + ""不能小于最小"" + pointName", InfoMessageType.Error,
            @"@requireCheckSize && MathFunc.AnyNumberBelow(maxPos, minPos)")]
        [OnValueChanged(nameof(OnBoundsChanged))]
        [JsonProperty]
        public TPointType maxPos;

        [LabelText(@"@sizeName")]
        [FoldoutGroup("信息", expanded: false)]
        [ShowInInspector, DisplayAsString]
        public virtual TPointType size { get; }

        [LabelText("中心点")]
        [FoldoutGroup("信息", expanded: false)]
        [ShowInInspector, DisplayAsString]
        public virtual TPointType pivot { get; }

        #region GUI

        protected virtual void OnBoundsChanged()
        {

        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overflow(TPointType minPosBorder, TPointType maxPosBorder)
        {
            return minPos.AnyNumberBelow(minPosBorder) || maxPos.AnyNumberAbove(maxPosBorder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TPointType pos)
        {
            return pos.AllNumberBetweenInclusive(minPos, maxPos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TPointType pos, out TPointType relativePos)
        {
            relativePos = GetRelativePos(pos);
            return Contains(pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsAll(params TPointType[] poses)
        {
            return poses.All(Contains);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsAny(params TPointType[] poses)
        {
            return poses.Any(Contains);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(KCubeShape<TPointType> kCube)
        {
            return ContainsAll(kCube.minPos, kCube.maxPos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsBy(KCubeShape<TPointType> kCube)
        {
            return kCube.Contains(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TPointType GetRelativePos(TPointType pos)
        {
            return pos.Subtract(minPos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TPointType Clamp(TPointType pos)
        {
            return pos.Clamp(minPos, maxPos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clamp(ref TPointType pos)
        {
            pos = Clamp(pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TPointType, TPointType) Clamp(TPointType posA, TPointType posB)
        {
            return (Clamp(posA), Clamp(posB));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clamp(ref TPointType posA, ref TPointType posB)
        {
            Clamp(ref posA);
            Clamp(ref posB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClampedBy(KCubeShape<TPointType> kCube)
        {
            kCube.Clamp(ref minPos);
            kCube.Clamp(ref maxPos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TPointType GetRandomPoint()
        {
            return minPos.RandomRange(maxPos);
        }

        public abstract IEnumerable<TPointType> GetRandomPoints(int count);

        public virtual IEnumerable<TPointType> GetRandomPoints(float rate)
        {
            int count = (size.Products() * rate).Round();
            return GetRandomPoints(count);
        }

        public virtual IEnumerable<TPointType> GetAllPoints()
        {
            Note.note.Warning("GetALlPoints():此方法没有被覆盖，请勿调用");
            yield break;
        }

        public void Add(TPointType addend)
        {
            minPos = minPos.Add(addend);
            maxPos = maxPos.Add(addend);
        }

        public override string ToString()
        {
            return $"[{minPos}, {maxPos}]";
        }

        public abstract object Clone();
    }

    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public class RangeInteger : KCubeShape<int>
    {
        protected override string pointName => "值";

        protected override string sizeName => "长度";

        public override int size => Mathf.Max(maxPos - minPos + 1, 0);

        public override int pivot => (minPos + maxPos).Divide(2);

        public RangeInteger()
        {
            minPos = 0;
            maxPos = 0;
        }

        public RangeInteger(int length)
        {
            minPos = 0;
            maxPos = length - 1;
        }

        public RangeInteger(int min, int max)
        {
            this.minPos = min;
            this.maxPos = max;

            if (min > max)
            {
                MathFunc.Swap(ref min, ref max);
            }
        }

        public override IEnumerable<int> GetRandomPoints(int count)
        {
            return count.GenerateUniqueIntegers(minPos, maxPos);
        }

        public override IEnumerable<int> GetAllPoints()
        {
            return minPos.GetRangeOfPoints(maxPos);
        }

        public override object Clone()
        {
            return new RangeInteger(minPos, maxPos);
        }

        public IEnumerable<int> Stepped(int step = 1)
        {
            return minPos.GetSteppedPoints(maxPos, step);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryUnion(RangeInteger other)
        {
            if (other.minPos <= maxPos + 1 && other.minPos >= minPos)
            {
                maxPos = Mathf.Max(maxPos, other.maxPos);
                return true;
            }

            if(other.maxPos >= minPos - 1 && other.maxPos <= maxPos)
            {
                minPos = Mathf.Min(minPos, other.minPos);
                return true;
            }

            if (other.minPos <= minPos - 1 && other.maxPos >= maxPos + 1)
            {
                maxPos = Mathf.Max(maxPos, other.maxPos);
                minPos = Mathf.Min(minPos, other.minPos);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryUnion(int other)
        {
            if (other <= maxPos + 1 && other >= minPos - 1)
            {
                maxPos = Mathf.Max(maxPos, other);
                minPos = Mathf.Min(minPos, other);
                return true;
            }

            return false;
        }
    }


    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public class RangeFloat : KCubeShape<float>
    {
        protected override string pointName => "值";

        protected override string sizeName => "长度";

        public override float size => Mathf.Max(maxPos - minPos, 0f);

        public override float pivot => (minPos + maxPos) / 2f;

        public RangeFloat()
        {
            minPos = 0f;
            maxPos = 0f;
        }

        public RangeFloat(float length)
        {
            minPos = 0f;
            maxPos = length;
        }

        public RangeFloat(float min, float max)
        {
            this.minPos = min;
            this.maxPos = max;

            if (min > max)
            {
                MathFunc.Swap(ref min, ref max);
            }
        }

        public override IEnumerable<float> GetRandomPoints(int count)
        {
            return count.DoFuncNTimes(GetRandomPoint);
        }

        public override object Clone()
        {
            return new RangeFloat(minPos, maxPos);
        }
    }


    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public class RectangleInt : KCubeShape<Vector2Int>
    {
        public int minX => minPos.x;
        public int minY => minPos.y;

        public int maxX => maxPos.x;
        public int maxY => maxPos.y;

        public override Vector2Int size => new(width, height);

        public int width => maxX - minX + 1;
        public int height => maxY - minY + 1;

        public override Vector2Int pivot => (maxPos + minPos).Divide(2);

        [LabelText("包含的整数点总数")]
        [FoldoutGroup("信息", expanded: false)]
        [ShowInInspector, DisplayAsString]
        public int pointCounts => size.Products();

        public RangeInteger xPoints => new(minX, maxX);
        public RangeInteger yPoints => new(minY, maxY);

        public RectangleInt()
        {
            this.minPos = Vector2Int.zero;
            this.maxPos = Vector2Int.zero;
        }

        public RectangleInt(Vector2Int minPos, Vector2Int maxPos)
        {
            this.minPos = minPos;
            this.maxPos = maxPos;
        }

        public RectangleInt(Vector2Int size)
        {
            minPos = Vector2Int.zero;
            maxPos = size - Vector2Int.one;
            if (size.AnyNumberBelow(0))
            {
                maxPos = -Vector2Int.one;
            }
        }

        public override IEnumerable<Vector2Int> GetRandomPoints(int count)
        {
            return count.GenerateUniqueVector2Ints(minPos, maxPos);
        }

        public override IEnumerable<Vector2Int> GetAllPoints()
        {
            return minPos.GetRectangleOfPoints(maxPos);
        }

        public override string ToString()
        {
            return $"{{minPos:{minPos}, maxPos:{maxPos}, size:{size}}}";
        }

        public override object Clone()
        {
            return new RectangleInt(minPos, maxPos);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public class RectangleFloat : KCubeShape<Vector2>
    {
        public float minX => minPos.x;
        public float minY => minPos.y;

        public float maxX => maxPos.x;
        public float maxY => maxPos.y;

        public override Vector2 size => new(width, height);

        public float width => maxX - minX;
        public float height => maxY - minY;

        public override Vector2 pivot => (maxPos + minPos) / 2f;

        [LabelText("包含的整数点总数")]
        [FoldoutGroup("信息", expanded: false)]
        [ShowInInspector, DisplayAsString]
        public float pointCounts => size.Products();

        public RangeFloat xPoints => new(minX, maxX);
        public RangeFloat yPoints => new(minY, maxY);

        public RectangleFloat()
        {
            this.minPos = Vector2Int.zero;
            this.maxPos = Vector2Int.zero;
        }

        public RectangleFloat(Vector2 minPos, Vector2 maxPos)
        {
            this.minPos = minPos;
            this.maxPos = maxPos;
        }

        public RectangleFloat(Vector2 size)
        {
            minPos = Vector2.zero;
            maxPos = size;
            if (size.AnyNumberBelow(0))
            {
                maxPos = Vector2.zero;
            }
        }

        public override IEnumerable<Vector2> GetRandomPoints(int count)
        {
            return count.DoFuncNTimes(GetRandomPoint);
        }

        public override string ToString()
        {
            return $"{{minPos:{minPos}, maxPos:{maxPos}, size:{size}}}";
        }

        public override object Clone()
        {
            return new RectangleFloat(minPos, maxPos);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public class CubeInt : KCubeShape<Vector3Int>
    {

        public int minX => minPos.x;
        public int minY => minPos.y;
        public int minZ => minPos.z;

        public int maxX => maxPos.x;
        public int maxY => maxPos.y;
        public int maxZ => maxPos.z;

        public override Vector3Int size => new(width, height, length);

        public int width => maxX - minX + 1;
        public int height => maxY - minY + 1;
        public int length => maxZ - minZ + 1;

        public override Vector3Int pivot => (minPos + maxPos).Divide(2);

        public int maxRadius => GetMinDistanceToBounds(pivot);

        public Vector2Int pivotXY => xyPoints.pivot;
        public Vector2Int pivotXZ => xzPoints.pivot;
        public Vector2Int pivotYZ => yzPoints.pivot;

        [LabelText("包含整数点的总数")]
        [FoldoutGroup("信息", expanded: false)]
        [ShowInInspector, DisplayAsString]
        public int pointCounts => size.Products();

        public RangeInteger xPoints => new(minX, maxX);
        public RangeInteger yPoints => new(minY, maxY);
        public RangeInteger zPoints => new(minZ, maxZ);

        public RectangleInt xyPoints => new(new(minX, minY), new(maxX, maxY));
        public RectangleInt xzPoints => new(new(minX, minZ), new(maxX, maxZ));
        public RectangleInt yzPoints => new(new(minY, minZ), new(maxY, maxZ));

        public CubeInt()
        {
            minPos = Vector3Int.zero;
            maxPos = Vector3Int.zero;
        }

        public CubeInt(Vector3Int size)
        {
            minPos = Vector3Int.zero;
            maxPos = size - Vector3Int.one;
            if (size.AnyNumberBelow(0))
            {
                Note.note.Warning("size参数不能有小于0的数");
                maxPos = new(-1, -1, -1);
            }

        }

        public CubeInt((Vector3Int, Vector3Int) minMaxPos) : this(minMaxPos.Item1, minMaxPos.Item2)
        {

        }

        public CubeInt(Vector3Int minPos, Vector3Int maxPos)
        {
            this.minPos = minPos;
            this.maxPos = maxPos;

            if (maxX < minX) minPos.SwapX(ref maxPos);
            if (maxY < minY) minPos.SwapY(ref maxPos);
            if (maxZ < minZ) minPos.SwapZ(ref maxPos);
        }

        public CubeInt(int minX, int minY, int minZ, int maxX, int maxY, int maxZ) : this(new(minX, minY, minZ), new(maxX, maxY, maxZ)) { }

        public override IEnumerable<Vector3Int> GetRandomPoints(int count)
        {
            return count.GenerateUniqueVector3Ints(minPos, maxPos);
        }

        public override IEnumerable<Vector3Int> GetAllPoints()
        {
            return minPos.GetCubeOfPoints(maxPos);
        }

        public IEnumerable<Vector3Int> GetCubeOfPoints(Vector3Int start, Vector3Int end)
        {
            Clamp(ref start, ref end);

            return start.GetCubeOfPoints(end);
        }

        public IEnumerable<Vector3Int> GetCircleOfPoints(Vector3Int pivot, float radius, DistanceType distanceType = DistanceType.Manhattan, PlaneType planeType = PlaneType.XY)
        {
            int maxCheckRadius = GetMinDistanceToBounds(pivot);

            if (radius < maxCheckRadius + 1)
            {
                foreach (var pos in pivot.GetCircleOfPoints(radius, distanceType, planeType))
                {
                    yield return pos;
                }
            }
            else
            {
                foreach (var pos in pivot.GetCircleOfPoints(radius, distanceType, planeType))
                {
                    if (Contains(pos))
                    {
                        yield return pos;
                    }
                }
            }
        }

        public int GetMinDistanceToBounds(Vector3Int point)
        {
            int minXDistance = point.x.MinDistance(maxX, minX);

            if (minXDistance == 0)
            {
                return 0;
            }

            int minYDistance = point.y.MinDistance(maxY, minY);

            if (minYDistance == 0)
            {
                return 0;
            }

            int minZDistance = point.z.MinDistance(minZ, maxZ);

            if (minZDistance == 0)
            {
                return 0;
            }

            return Mathf.Min(minXDistance, minYDistance, minZDistance);
        }

        public override string ToString()
        {
            return $"{{minPos:{minPos}, maxPos:{maxPos}, size:{size}}}";
        }

        public override object Clone()
        {
            return new CubeInt(minPos, maxPos);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public class CubeFloat : KCubeShape<Vector3>
    {

        public float minX => minPos.x;
        public float minY => minPos.y;
        public float minZ => minPos.z;

        public float maxX => maxPos.x;
        public float maxY => maxPos.y;
        public float maxZ => maxPos.z;

        public override Vector3 size => new(width, height, length);

        public float width => maxX - minX;
        public float height => maxY - minY;
        public float length => maxZ - minZ;

        public override Vector3 pivot => (minPos + maxPos) / 2;

        public Vector2 pivotXY => xyPoints.pivot;
        public Vector2 pivotXZ => xzPoints.pivot;
        public Vector2 pivotYZ => yzPoints.pivot;

        public RangeFloat xPoints => new(minX, maxX);
        public RangeFloat yPoints => new(minY, maxY);
        public RangeFloat zPoints => new(minZ, maxZ);

        public RectangleFloat xyPoints => new(new(minX, minY), new(maxX, maxY));
        public RectangleFloat xzPoints => new(new(minX, minZ), new(maxX, maxZ));
        public RectangleFloat yzPoints => new(new(minY, minZ), new(maxY, maxZ));

        public CubeFloat() : this(new(0, 0, 0), new(0, 0, 0)) { }

        public CubeFloat(Vector3 size)
        {
            minPos = Vector3.zero;
            maxPos = size;
            if (size.AnyNumberBelow(0))
            {
                Note.note.Warning("size参数不能有小于0的数");
                maxPos = Vector3.zero;
            }

        }

        public CubeFloat((Vector3, Vector3) minMaxPos) : this(minMaxPos.Item1, minMaxPos.Item2)
        {

        }

        public CubeFloat(Vector3 minPos, Vector3 maxPos)
        {
            this.minPos = minPos;
            this.maxPos = maxPos;

            if (maxX < minX) minPos.SwapX(ref maxPos);
            if (maxY < minY) minPos.SwapY(ref maxPos);
            if (maxZ < minZ) minPos.SwapZ(ref maxPos);
        }

        public CubeFloat(float minX, float minY, float minZ, float maxX, float maxY, float maxZ) :
            this(new(minX, minY, minZ), new(maxX, maxY, maxZ))
        { }

        public override IEnumerable<Vector3> GetRandomPoints(int count)
        {
            return count.DoFuncNTimes(GetRandomPoint);
        }

        public override string ToString()
        {
            return $"{{minPos:{minPos}, maxPos:{maxPos}, size:{size}}}";
        }

        public override object Clone()
        {
            return new CubeFloat(minPos, maxPos);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public class TesseractFloat : KCubeShape<Vector4>
    {

        public float minX => minPos.x;
        public float minY => minPos.y;
        public float minZ => minPos.z;
        public float minW => minPos.w;

        public float maxX => maxPos.x;
        public float maxY => maxPos.y;
        public float maxZ => maxPos.z;
        public float maxW => maxPos.w;

        public override Vector4 size => new(xLength, yLength, zLength, wLength);

        public float xLength => maxX - minX;
        public float yLength => maxY - minY;
        public float zLength => maxZ - minZ;

        public float wLength => maxW - minW;

        public override Vector4 pivot => (minPos + maxPos) / 2;

        //public Vector2 pivotXY => xyPoints.pivot;
        //public Vector2 pivotXZ => xzPoints.pivot;
        //public Vector2 pivotYZ => yzPoints.pivot;

        //public RangeFloat xPoints => new(minX, maxX);
        //public RangeFloat yPoints => new(minY, maxY);
        //public RangeFloat zPoints => new(minZ, maxZ);

        //public RectangleFloat xyPoints => new(new(minX, minY), new(maxX, maxY));
        //public RectangleFloat xzPoints => new(new(minX, minZ), new(maxX, maxZ));
        //public RectangleFloat yzPoints => new(new(minY, minZ), new(maxY, maxZ));

        public TesseractFloat() : this(new(0, 0, 0, 0), new(0, 0, 0, 0)) { }

        public TesseractFloat(Vector4 size)
        {
            minPos = Vector4.zero;
            maxPos = size;
            if (size.AnyNumberBelow(0))
            {
                Note.note.Warning("size参数不能有小于0的数");
                maxPos = Vector4.zero;
            }
        }

        public TesseractFloat((Vector4, Vector4) minMaxPos) : this(minMaxPos.Item1, minMaxPos.Item2)
        {

        }

        public TesseractFloat(Vector4 minPos, Vector4 maxPos)
        {
            this.minPos = minPos;
            this.maxPos = maxPos;

            if (maxX < minX) minPos.SwapX(ref maxPos);
            if (maxY < minY) minPos.SwapY(ref maxPos);
            if (maxZ < minZ) minPos.SwapZ(ref maxPos);
            if (maxW < minW) minPos.SwapW(ref maxPos);
        }

        public TesseractFloat(float minX, float minY, float minZ, float minW, float maxX, float maxY, float maxZ, float maxW) :
            this(new(minX, minY, minZ, minW), new(maxX, maxY, maxZ, maxW))
        { }

        public override IEnumerable<Vector4> GetRandomPoints(int count)
        {
            return count.DoFuncNTimes(GetRandomPoint);
        }

        public override string ToString()
        {
            return $"{{minPos:{minPos}, maxPos:{maxPos}, size:{size}}}";
        }

        public override object Clone()
        {
            return new TesseractFloat(minPos, maxPos);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public class ColorRange : KCubeShape<Color>
    {
        protected override string pointName => "颜色";

        protected override bool requireCheckSize => false;

        public float minR => minPos.r.Min(maxPos.r);
        public float minG => minPos.g.Min(maxPos.g);
        public float minB => minPos.b.Min(maxPos.b);
        public float minA => minPos.a.Min(maxPos.a);

        public float maxR => maxPos.r.Max(minPos.r);
        public float maxG => maxPos.g.Max(minPos.g);
        public float maxB => maxPos.b.Max(minPos.b);
        public float maxA => maxPos.a.Max(minPos.a);

        public override Color size => new(rLength, gLength, bLength, aLength);

        public float rLength => (maxR - minR).Abs();
        public float gLength => (maxG - minG).Abs();
        public float bLength => (maxB - minB).Abs();

        public float aLength => (maxA - minA).Abs();

        public override Color pivot => (minPos + maxPos) / 2;

        //public Vector2 pivotXY => xyPoints.pivot;
        //public Vector2 pivotXZ => xzPoints.pivot;
        //public Vector2 pivotYZ => yzPoints.pivot;

        //public RangeFloat xPoints => new(minX, maxX);
        //public RangeFloat yPoints => new(minY, maxY);
        //public RangeFloat zPoints => new(minZ, maxZ);

        //public RectangleFloat xyPoints => new(new(minX, minY), new(maxX, maxY));
        //public RectangleFloat xzPoints => new(new(minX, minZ), new(maxX, maxZ));
        //public RectangleFloat yzPoints => new(new(minY, minZ), new(maxY, maxZ));

        public ColorRange() : this(new(0, 0, 0, 0), new(0, 0, 0, 0)) { }

        public ColorRange(Color size)
        {
            minPos = ColorFunc.zero;
            maxPos = size;
            if (size.AnyNumberBelow(0))
            {
                Note.note.Warning("size参数不能有小于0的数");
                maxPos = ColorFunc.zero;
            }
        }

        public ColorRange((Color, Color) minMaxPos) : this(minMaxPos.Item1, minMaxPos.Item2)
        {

        }

        public ColorRange(Color minPos, Color maxPos)
        {
            this.minPos = minPos;
            this.maxPos = maxPos;

            if (maxR < minR) minPos.SwapRed(ref maxPos);
            if (maxG < minG) minPos.SwapGreen(ref maxPos);
            if (maxB < minB) minPos.SwapBlue(ref maxPos);
            if (maxA < minA) minPos.SwapAlpha(ref maxPos);
        }

        public ColorRange(float minX, float minY, float minZ, float minW, float maxX, float maxY, float maxZ, float maxW) :
            this(new(minX, minY, minZ, minW), new(maxX, maxY, maxZ, maxW))
        { }

        public override IEnumerable<Color> GetRandomPoints(int count)
        {
            return count.DoFuncNTimes(GetRandomPoint);
        }

        public override string ToString()
        {
            return $"{{minPos:{minPos}, maxPos:{maxPos}, size:{size}}}";
        }

        public override object Clone()
        {
            return new ColorRange(minPos, maxPos);
        }
    }


    #endregion

    #region ShapeSet

    [Serializable]
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    public class RangeIntegerSet
    {
        public enum FindStrategy
        {
            NormalNearest,
            BiggerFirst,
            SmallerFirst
        }

        [LabelText("最大值")]
        [ShowInInspector]
        public int maxPos => set.Count == 0 ? int.MinValue : set[^1].maxPos;
        [LabelText("最小值")]
        [ShowInInspector]
        public int minPos => set.Count == 0 ? int.MaxValue : set[0].minPos;

        [LabelText("集合")]
        [ShowInInspector]
        private readonly List<RangeInteger> set = new();

        public bool Contains(int pos)
        {
            return set.Any(range => range.Contains(pos));
        }

        [Button("随机添加点"), ShowIf("isDebugging")]
        public void RandomAddPoint(int count, int start, int end)
        {
            foreach (var pos in count.GenerateUniqueIntegers(start, end))
            {
                AddPoint(pos);
            }
        }

        [Button("添加点"), ShowIf("isDebugging")]
        public void AddPoint(int pos)
        {
            List<RangeInteger> toUnions = new();

            int i;
            for (i = 0; i < set.Count; i++)
            {
                var range = set[i];

                if (pos < range.minPos - 1)
                {
                    break;
                }
                if (pos >= range.minPos - 1 && pos <= range.maxPos)
                {
                    toUnions.Add(range);
                    break;
                }

                if (pos == range.maxPos + 1)
                {
                    toUnions.Add(range);
                }
            }

            if (toUnions.Count <= 0)
            {
                set.Insert(i, new RangeInteger(pos, pos));
            }
            else
            {
                var first = toUnions[0];

                if (first.TryUnion(pos) == false)
                {
                    Note.note.Error($"异常, pos:{pos}, first:{first}, set:{set}");
                }

                for (var index = 1; index < toUnions.Count; index++)
                {
                    var range = toUnions[index];

                    if (first.TryUnion(range) == false)
                    {
                        Note.note.Error($"异常, first:{first}, other:{range}, set:{set}");
                    }

                    set.Remove(range);
                }
            }
        }

        [Button("随机添加范围"), ShowIf("isDebugging")]
        public void RandomAddRange(int count, int start, int end)
        {
            foreach (var range in count.SeveralRandomRangeInteger(start, end))
            {
                AddRange(range);
            }
        }

        [Button("添加范围"), ShowIf("isDebugging")]
        public void AddRange(int start, int end)
        {
            AddRange(new(start, end));
        }

        public void AddRange(RangeInteger other)
        {
            List<RangeInteger> toUnions = new();

            int i;
            for (i = 0; i < set.Count; i++)
            {
                var range = set[i];

                if (range.minPos > other.maxPos + 1)
                {
                    break;
                }

                if (range.maxPos >= other.minPos - 1)
                {
                    toUnions.Add(range);
                }
            }

            if (toUnions.Count <= 0)
            {
                set.Insert(i, other);
            }
            else
            {
                var first = toUnions[0];

                if (first.TryUnion(other) == false)
                {
                    Note.note.Error($"异常, other:{other}, first:{first}, set:{set}");
                }

                for (var index = 1; index < toUnions.Count; index++)
                {
                    var range = toUnions[index];

                    if (first.TryUnion(range) == false)
                    {
                        Note.note.Error($"异常, first:{first}, other:{range}, set:{set}");
                    }

                    set.Remove(range);
                }
            }
        }

        [Button("找最近的最大点"), ShowIf("isDebugging")]
        public bool TryFindNearestMaxPos(int origin, out int result,
            FindStrategy whileContained = FindStrategy.NormalNearest, 
            FindStrategy whileNotContained = FindStrategy.NormalNearest)
        {
            if (set.Count <= 0)
            {
                Note.note.Warning("set为空");
                result = 0;
                return false;
            }

            var strategy = Contains(origin) switch
            {
                true => whileContained,
                false => whileNotContained
            };

            switch (strategy)
            {
                case FindStrategy.NormalNearest:

                    int nearestMaxPos = 0;
                    int minDistance = int.MaxValue;
                    foreach (var range in set)
                    {
                        if (range.maxPos >= origin)
                        {
                            if ((range.maxPos - origin) < minDistance)
                            {
                                nearestMaxPos = range.maxPos;
                            }

                            break;
                        }

                        int distance = origin - range.maxPos;

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestMaxPos = range.maxPos;
                        }
                    }

                    result = nearestMaxPos;
                    return true;

                case FindStrategy.BiggerFirst:

                    int smallerMaxPos = 0;
                    foreach (var range in set)
                    {
                        if (range.maxPos >= origin)
                        {
                            result = range.maxPos;
                            return true;
                        }

                        smallerMaxPos = range.maxPos;
                    }

                    result = smallerMaxPos;
                    return true;

                case FindStrategy.SmallerFirst:

                    int biggerMaxPos = 0;
                    for (var i = set.Count - 1; i >= 0; i--)
                    {
                        var range = set[i];
                        if (range.maxPos <= origin)
                        {
                            result = range.maxPos;
                            return true;
                        }

                        biggerMaxPos = range.maxPos;
                    }

                    result = biggerMaxPos;
                    return true;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //public void RemovePoint(int point)
        //{
        //    RangeInteger toCut = null;
        //    foreach (var range in set)
        //    {

        //    }
        //}

        public override string ToString()
        {
            return set.ToString(",");
        }
    }

    #endregion
}