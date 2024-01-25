﻿using System;

namespace KSoft.IO
{
	partial class EndianReader
	{
		#region VirtualAddressTranslation
		Memory.VirtualAddressTranslationStack mVAT;

		/// <summary>Verify the state of the VAT (is it initialized?)</summary>
		void VerifyVAT()
		{
			if (mVAT == null)
				throw new InvalidOperationException("VAT uninitialized");
		}
		/// <summary>Initialize the VAT with a specific handle size and initial table capacity</summary>
		/// <param name="vaSize">Handle size</param>
		/// <param name="translationCapacity">The initial table capacity</param>
		public void VirtualAddressTranslationInitialize(Shell.ProcessorSize vaSize, int translationCapacity = 0)
		{
			if (mVAT == null)
			{
				mVAT = new Memory.VirtualAddressTranslationStack(vaSize, translationCapacity);
				mVAT.PushNull(); // implicitly use null as our initial VA translator
			}
		}
		/// <summary>Push a PA into to the VAT table, setting the current PA in the process</summary>
		/// <param name="physicalAddress">PA to push and to use as the VAT's current address</param>
		public void VirtualAddressTranslationPush(Values.PtrHandle physicalAddress)
		{
			VerifyVAT();

			mVAT.PushPhysicalAddress(physicalAddress);
		}
		/// <summary>Push the stream's position (as a physical address) into the VAT table</summary>
		public void VirtualAddressTranslationPushPosition()
		{
			VirtualAddressTranslationPush(PositionPtr);
		}
		/// <summary>Increase the current address (PA) by a relative offset</summary>
		/// <param name="relativeOffset">Offset, relative to the current address</param>
		public void VirtualAddressTranslationIncrease(Values.PtrHandle relativeOffset)
		{
			VerifyVAT();

			mVAT.PushPhysicalAddressOffset(relativeOffset);
		}
		/// <summary>Pop and return the current address (PA) in the VAT table</summary>
		/// <returns>The VAT's current address value before this call</returns>
		public Values.PtrHandle VirtualAddressTranslationPop()
		{
			VerifyVAT();

			if (mVAT.Count == 1)
				throw new InvalidOperationException("Pop underflow");

			return mVAT.PopPhysicalAddress();
		}
		#endregion
	};

	partial class EndianWriter
	{
		#region VirtualAddressTranslation
		Memory.VirtualAddressTranslationStack mVAT;

		/// <summary>Verify the state of the VAT (is it initialized?)</summary>
		void VerifyVAT()
		{
			if (mVAT == null)
				throw new InvalidOperationException("VAT uninitialized");
		}
		/// <summary>Initialize the VAT with a specific handle size and initial table capacity</summary>
		/// <param name="vaSize">Handle size</param>
		/// <param name="translationCapacity">The initial table capacity</param>
		public void VirtualAddressTranslationInitialize(Shell.ProcessorSize vaSize, int translationCapacity = 0)
		{
			if (mVAT == null)
			{
				mVAT = new Memory.VirtualAddressTranslationStack(vaSize, translationCapacity);
				mVAT.PushNull(); // implicitly use null as our initial VA translator
			}
		}
		/// <summary>Push a PA into to the VAT table, setting the current PA in the process</summary>
		/// <param name="physicalAddress">PA to push and to use as the VAT's current address</param>
		public void VirtualAddressTranslationPush(Values.PtrHandle physicalAddress)
		{
			VerifyVAT();

			mVAT.PushPhysicalAddress(physicalAddress);
		}
		/// <summary>Push the stream's position (as a physical address) into the VAT table</summary>
		public void VirtualAddressTranslationPushPosition()
		{
			VirtualAddressTranslationPush(PositionPtr);
		}
		/// <summary>Increase the current address (PA) by a relative offset</summary>
		/// <param name="relativeOffset">Offset, relative to the current address</param>
		public void VirtualAddressTranslationIncrease(Values.PtrHandle relativeOffset)
		{
			VerifyVAT();

			mVAT.PushPhysicalAddressOffset(relativeOffset);
		}
		/// <summary>Pop and return the current address (PA) in the VAT table</summary>
		/// <returns>The VAT's current address value before this call</returns>
		public Values.PtrHandle VirtualAddressTranslationPop()
		{
			VerifyVAT();

			if (mVAT.Count == 1)
				throw new InvalidOperationException("Pop underflow");

			return mVAT.PopPhysicalAddress();
		}
		#endregion
	};

}