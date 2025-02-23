﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AngouriMath.CPP.Exporting
{
    partial class Exports
    {
        [UnmanagedCallersOnly(EntryPoint = "op_entity_add")]
        public static NErrorCode Add(ObjRef left, ObjRef right, ref ObjRef res)
            => ExceptionEncode(ref res, (left, right),
                e => e.left.AsEntity + e.right.AsEntity
                );

        [UnmanagedCallersOnly(EntryPoint = "op_entity_sub")]
        public static NErrorCode Subtract(ObjRef left, ObjRef right, ref ObjRef res)
            => ExceptionEncode(ref res, (left, right),
                e => e.left.AsEntity - e.right.AsEntity
                );

        [UnmanagedCallersOnly(EntryPoint = "op_entity_mul")]
        public static NErrorCode Multiply(ObjRef left, ObjRef right, ref ObjRef res)
            => ExceptionEncode(ref res, (left, right),
                e => e.left.AsEntity * e.right.AsEntity
                );

        [UnmanagedCallersOnly(EntryPoint = "op_entity_div")]
        public static NErrorCode Divide(ObjRef left, ObjRef right, ref ObjRef res)
            => ExceptionEncode(ref res, (left, right),
                e => e.left.AsEntity / e.right.AsEntity
                );

        [UnmanagedCallersOnly(EntryPoint = "op_entity_less")]
        public static NErrorCode Less(ObjRef left, ObjRef right, ref ObjRef res)
            => ExceptionEncode(ref res, (left, right),
                e => e.left.AsEntity < e.right.AsEntity
                );

        [UnmanagedCallersOnly(EntryPoint = "op_entity_greater")]
        public static NErrorCode Greater(ObjRef left, ObjRef right, ref ObjRef res)
            => ExceptionEncode(ref res, (left, right),
                e => e.left.AsEntity > e.right.AsEntity
                );

        [UnmanagedCallersOnly(EntryPoint = "op_entity_less_or_equal")]
        public static NErrorCode LessOrEqual(ObjRef left, ObjRef right, ref ObjRef res)
            => ExceptionEncode(ref res, (left, right),
                e => e.left.AsEntity <= e.right.AsEntity
                );

        [UnmanagedCallersOnly(EntryPoint = "op_entity_greater_or_equal")]
        public static NErrorCode GreaterOrEqual(ObjRef left, ObjRef right, ref ObjRef res)
            => ExceptionEncode(ref res, (left, right),
                e => e.left.AsEntity >= e.right.AsEntity
                );

        [UnmanagedCallersOnly(EntryPoint = "op_entity_equal")]
        public static NErrorCode OpEqual(ObjRef left, ObjRef right, ref NativeBool res)
            => ExceptionEncode(ref res, (left, right),
                e => e.left.AsEntity == e.right.AsEntity
                );
    }
}
