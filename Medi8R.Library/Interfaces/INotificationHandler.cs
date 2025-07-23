namespace Medi8R.Library.Interfaces
{
    public interface INotificationHandler<in TNotification>
        where TNotification : INotification
    {
        Task Handle(TNotification notification);
    }
}