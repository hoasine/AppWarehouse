// Decompiled with JetBrains decompiler
// Type: ApiDefinition.Messaging
// Assembly: XamarinDataManLibrary, Version=2.2.3.2200, Culture=neutral, PublicKeyToken=null
// MVID: 4FBA3308-34ED-49D0-BCE6-80E794F8239E
// Assembly location: C:\Users\hoa.tran\Desktop\File\ScanCogNex IOS\XamarinDataManLibrary.dll

using CoreGraphics;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ApiDefinition
{
  internal class Messaging
  {
    internal static Assembly this_assembly = typeof (Messaging).Assembly;
    private const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend(IntPtr receiever, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper(IntPtr receiever, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_IntPtr(
      IntPtr receiever,
      IntPtr selector,
      IntPtr arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_IntPtr(
      IntPtr receiever,
      IntPtr selector,
      IntPtr arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern uint UInt32_objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern uint UInt32_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern ulong UInt64_objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern ulong UInt64_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern int int_objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern int int_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern long Int64_objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern long Int64_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_int(IntPtr receiver, IntPtr selector, int arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_int(
      IntPtr receiver,
      IntPtr selector,
      int arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_Int64(IntPtr receiver, IntPtr selector, long arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_Int64(
      IntPtr receiver,
      IntPtr selector,
      long arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern bool bool_objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern bool bool_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern double Double_objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern double Double_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_Double(
      IntPtr receiver,
      IntPtr selector,
      double arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_Double(
      IntPtr receiver,
      IntPtr selector,
      double arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_IntPtr_int_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      int arg2,
      IntPtr arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_IntPtr_int_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      int arg2,
      IntPtr arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_int_int_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2,
      IntPtr arg3,
      IntPtr arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_int_int_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2,
      IntPtr arg3,
      IntPtr arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_Int64_Int64_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2,
      IntPtr arg3,
      IntPtr arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_Int64_Int64_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2,
      IntPtr arg3,
      IntPtr arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_int_int_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2,
      IntPtr arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_int_int_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2,
      IntPtr arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_Int64_Int64_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2,
      IntPtr arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_Int64_Int64_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2,
      IntPtr arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_int_int_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2,
      IntPtr arg3,
      IntPtr arg4,
      IntPtr arg5);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_int_int_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2,
      IntPtr arg3,
      IntPtr arg4,
      IntPtr arg5);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_Int64_Int64_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2,
      IntPtr arg3,
      IntPtr arg4,
      IntPtr arg5);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_Int64_Int64_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2,
      IntPtr arg3,
      IntPtr arg4,
      IntPtr arg5);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_int_int_IntPtr_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2,
      IntPtr arg3,
      IntPtr arg4,
      IntPtr arg5,
      IntPtr arg6);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_int_int_IntPtr_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2,
      IntPtr arg3,
      IntPtr arg4,
      IntPtr arg5,
      IntPtr arg6);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_Int64_Int64_IntPtr_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2,
      IntPtr arg3,
      IntPtr arg4,
      IntPtr arg5,
      IntPtr arg6);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_Int64_Int64_IntPtr_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2,
      IntPtr arg3,
      IntPtr arg4,
      IntPtr arg5,
      IntPtr arg6);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern bool bool_objc_msgSend_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern bool bool_objc_msgSendSuper_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern bool bool_objc_msgSend_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern bool bool_objc_msgSendSuper_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern bool bool_objc_msgSend_IntPtr_IntPtr_Double_bool_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2,
      double arg3,
      bool arg4,
      IntPtr arg5);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern bool bool_objc_msgSendSuper_IntPtr_IntPtr_Double_bool_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2,
      double arg3,
      bool arg4,
      IntPtr arg5);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_bool(IntPtr receiver, IntPtr selector, bool arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_bool(
      IntPtr receiver,
      IntPtr selector,
      bool arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_int_int_int_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2,
      int arg3,
      IntPtr arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_int_int_int_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2,
      int arg3,
      IntPtr arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_Int64_Int64_Int64_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2,
      long arg3,
      IntPtr arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_Int64_Int64_Int64_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2,
      long arg3,
      IntPtr arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_IntPtr_IntPtr_int(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2,
      int arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_IntPtr_IntPtr_int(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2,
      int arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_IntPtr_int_nint_nint(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      int arg2,
      nint arg3,
      nint arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_IntPtr_int_nint_nint(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      int arg2,
      nint arg3,
      nint arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_IntPtr_Int64_nint_nint(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      long arg2,
      nint arg3,
      nint arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_IntPtr_Int64_nint_nint(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      long arg2,
      nint arg3,
      nint arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_IntPtr_byte(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      byte arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_IntPtr_byte(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      byte arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_UInt32(IntPtr receiver, IntPtr selector, uint arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_UInt32(
      IntPtr receiver,
      IntPtr selector,
      uint arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_UInt64(
      IntPtr receiver,
      IntPtr selector,
      ulong arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_UInt64(
      IntPtr receiver,
      IntPtr selector,
      ulong arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_int_int(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_int_int(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      int arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_Int64_Int64(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_Int64_Int64(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      long arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_UInt32_bool_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      uint arg1,
      bool arg2,
      IntPtr arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_UInt32_bool_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      uint arg1,
      bool arg2,
      IntPtr arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_UInt64_bool_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      ulong arg1,
      bool arg2,
      IntPtr arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_UInt64_bool_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      ulong arg1,
      bool arg2,
      IntPtr arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_UInt32_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      uint arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_UInt32_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      uint arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_UInt64_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      ulong arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_UInt64_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      ulong arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_bool_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      bool arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_bool_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      bool arg1,
      IntPtr arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern int int_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern int int_objc_msgSendSuper_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern long Int64_objc_msgSend_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern long Int64_objc_msgSendSuper_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern bool bool_objc_msgSend_int(IntPtr receiver, IntPtr selector, int arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern bool bool_objc_msgSendSuper_int(
      IntPtr receiver,
      IntPtr selector,
      int arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern bool bool_objc_msgSend_Int64(IntPtr receiver, IntPtr selector, long arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern bool bool_objc_msgSendSuper_Int64(
      IntPtr receiver,
      IntPtr selector,
      long arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern bool bool_objc_msgSend_int_bool(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      bool arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern bool bool_objc_msgSendSuper_int_bool(
      IntPtr receiver,
      IntPtr selector,
      int arg1,
      bool arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern bool bool_objc_msgSend_Int64_bool(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      bool arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern bool bool_objc_msgSendSuper_Int64_bool(
      IntPtr receiver,
      IntPtr selector,
      long arg1,
      bool arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_bool_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2,
      IntPtr arg3,
      IntPtr arg4,
      bool arg5,
      IntPtr arg6,
      IntPtr arg7,
      IntPtr arg8,
      IntPtr arg9,
      IntPtr arg10);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_IntPtr_bool_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2,
      IntPtr arg3,
      IntPtr arg4,
      bool arg5,
      IntPtr arg6,
      IntPtr arg7,
      IntPtr arg8,
      IntPtr arg9,
      IntPtr arg10);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2,
      IntPtr arg3,
      IntPtr arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_IntPtr(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      IntPtr arg2,
      IntPtr arg3,
      IntPtr arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern CGPoint CGPoint_objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern CGPoint CGPoint_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend_stret")]
    public static extern void CGPoint_objc_msgSend_stret(
      out CGPoint retval,
      IntPtr receiver,
      IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper_stret")]
    public static extern void CGPoint_objc_msgSendSuper_stret(
      out CGPoint retval,
      IntPtr receiver,
      IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_CGPoint(
      IntPtr receiver,
      IntPtr selector,
      CGPoint arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_CGPoint(
      IntPtr receiver,
      IntPtr selector,
      CGPoint arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_float_float_float_float_float_float_float_float(
      IntPtr receiver,
      IntPtr selector,
      float arg1,
      float arg2,
      float arg3,
      float arg4,
      float arg5,
      float arg6,
      float arg7,
      float arg8);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_float_float_float_float_float_float_float_float(
      IntPtr receiver,
      IntPtr selector,
      float arg1,
      float arg2,
      float arg3,
      float arg4,
      float arg5,
      float arg6,
      float arg7,
      float arg8);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_float(IntPtr receiver, IntPtr selector, float arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_float(
      IntPtr receiver,
      IntPtr selector,
      float arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_CGPoint_int_int(
      IntPtr receiver,
      IntPtr selector,
      CGPoint arg1,
      int arg2,
      int arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_CGPoint_int_int(
      IntPtr receiver,
      IntPtr selector,
      CGPoint arg1,
      int arg2,
      int arg3);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_CGPoint_int_int_int(
      IntPtr receiver,
      IntPtr selector,
      CGPoint arg1,
      int arg2,
      int arg3,
      int arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_CGPoint_int_int_int(
      IntPtr receiver,
      IntPtr selector,
      CGPoint arg1,
      int arg2,
      int arg3,
      int arg4);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_IntPtr_bool(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      bool arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_IntPtr_bool(
      IntPtr receiver,
      IntPtr selector,
      IntPtr arg1,
      bool arg2);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern byte byte_objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern byte byte_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend_byte(IntPtr receiver, IntPtr selector, byte arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern void void_objc_msgSendSuper_byte(
      IntPtr receiver,
      IntPtr selector,
      byte arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern float float_objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern float float_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_byte(
      IntPtr receiver,
      IntPtr selector,
      byte arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_byte(
      IntPtr receiver,
      IntPtr selector,
      byte arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr IntPtr_objc_msgSend_int(
      IntPtr receiver,
      IntPtr selector,
      int arg1);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
    public static extern IntPtr IntPtr_objc_msgSendSuper_int(
      IntPtr receiver,
      IntPtr selector,
      int arg1);
  }
}
