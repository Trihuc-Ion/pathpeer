namespace PathPeer.Application.Interfaces.Services;

public interface IReviewJobService
{
    Task ProcessExpiredReviewsAsync();
}