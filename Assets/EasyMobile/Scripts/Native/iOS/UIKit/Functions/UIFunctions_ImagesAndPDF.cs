#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.Internal.iOS.UIKit;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.UIKit
{
    internal static partial class UIFunctions
    {
        #region Images And PDF

        /// <summary>
        /// Adds the specified image to the user’s Camera Roll album.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public static void UIImageWriteToSavedPhotosAlbum(UIImage image, Action<UIImage, NSError> completionHandler)
        {
            Util.NullArgumentTest(image);

            C.UIKit_UIImageWriteToSavedPhotosAlbum(
                image.ToPointer(),
                WriteToSavedPhotosAlbumCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Returns the data for the specified image in JPEG format.
        /// </summary>
        /// <returns>The image JPEG representation.</returns>
        /// <param name="image">Image.</param>
        /// <param name="compressionQuality">Compression quality.</param>
        public static NSData UIImageJPEGRepresentation(UIImage image, float compressionQuality)
        {
            Util.NullArgumentTest(image);

            IntPtr ptr = C.UIKit_UIImageJPEGRepresentation(image.ToPointer(), compressionQuality);
            NSData data = new NSData(ptr);
            CoreFoundation.CFFunctions.CFRelease(ptr);
            return data;
        }

        /// <summary>
        /// Returns the data for the specified image in PNG format.
        /// </summary>
        /// <returns>The image PNG representation.</returns>
        /// <param name="image">Image.</param>
        public static NSData UIImagePNGRepresentation(UIImage image)
        {
            Util.NullArgumentTest(image);

            IntPtr ptr = C.UIKit_UIImagePNGRepresentation(image.ToPointer());
            NSData data = new NSData(ptr);
            CoreFoundation.CFFunctions.CFRelease(ptr);
            return data;
        }

        #endregion

        #region Internal Callbacks

        [MonoPInvokeCallback(typeof(C.WriteToSavedPhotosAlbumCallback))]
        private static void WriteToSavedPhotosAlbumCallback(
            /* UIImage */IntPtr imagePtr, /* InteropNSError */IntPtr errorPtr, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            var image = InteropObjectFactory<UIImage>.FromPointer(imagePtr, p => new UIImage(p));
            var error = PInvokeUtil.IsNotNull(errorPtr) ? new NSError(errorPtr) : null;

            PInvokeCallbackUtil.PerformInternalCallback(
                "UIKitFunctions#WriteToSavedPhotosAlbumCallback",
                PInvokeCallbackUtil.Type.Temporary,
                image, error, secondaryCallback);
        }

        #endregion

        #region C wrapper

        private static partial class C
        {
            internal delegate void WriteToSavedPhotosAlbumCallback(
            /* UIImage */IntPtr image,
            /* NSError */IntPtr error,
            IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void UIKit_UIImageWriteToSavedPhotosAlbum(
                /* UIImage */IntPtr image, 
                             WriteToSavedPhotosAlbumCallback callback, 
                             IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern /* NSData */IntPtr UIKit_UIImageJPEGRepresentation(/* UIImage */IntPtr image, float compressionQuality);

            [DllImport("__Internal")]
            internal static extern /* NSData */IntPtr UIKit_UIImagePNGRepresentation(/* UIImage */IntPtr image);
        }

        #endregion
    }
}
#endif // UNITY_IOS