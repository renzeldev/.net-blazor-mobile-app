using System.ComponentModel.DataAnnotations;

namespace UmfaApp.Enums
{
    public enum NotValidatedReason
    {
        [Display(Name = "Incorrect login credentials.")] IncorrectDetails = 0,
        [Display(Name = "Device has not found.")] DeviceNotFound = 1,
        [Display(Name = "Device has not been approved.")] DeviceNotApproved = 2,
        [Display(Name = "User has been locked out.")] UserLockedOut = 3,
    }
}
