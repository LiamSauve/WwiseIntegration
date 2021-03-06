/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.8
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;
using System.Runtime.InteropServices;

public class EnvelopePoint : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal EnvelopePoint(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(EnvelopePoint obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~EnvelopePoint() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_EnvelopePoint(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

  public uint uPosition {
    set {
      AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uPosition_set(swigCPtr, value);
    } 
    get {
      uint ret = AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uPosition_get(swigCPtr);
      return ret;
    } 
  }

  public ushort uAttenuation {
    set {
      AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uAttenuation_set(swigCPtr, value);
    } 
    get {
      ushort ret = AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uAttenuation_get(swigCPtr);
      return ret;
    } 
  }

  public EnvelopePoint() : this(AkSoundEnginePINVOKE.CSharp_new_EnvelopePoint(), true) {

  }

}
