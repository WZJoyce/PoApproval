using FluentAssertions;
using Moq;
using PoApproval.Api.Services;
using PoApproval.Domain.Advisory;
using PoApproval.Domain.Entities;
using PoApproval.Domain.Exceptions;
using PoApproval.Domain.Repositories;
using Xunit;

public class OrderRecommendationServiceTests
{
    [Fact]
    public async Task GetRecommendation_WhenOrderExists_CallsAdvisorWithHistory()
    {
        var order = new PurchaseOrder { Amount = 1500, CreatedBy = "alice" };


        var repo = new Mock<IPurchaseOrderRepository>();
        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        repo.Setup(r => r.GetRequesterHistoryAsync("alice", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RequesterHistory
            {
                Requester = "alice",
                TotalOrders = 5,
                AverageAmount = 1000,
                MaxAmount = 2000,
                RejectedCount = 1,
            });

        var advisor = new Mock<IApprovalAdvisor>();
        advisor.Setup(a => a.GetRecommendationAsync(
                It.IsAny<PurchaseOrder>(), It.IsAny<RequesterHistory>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdvisorRecommendation
            {
                Verdict = AdvisorVerdict.LikelyApprove,
                Confidence = 0.9,
                Summary = "ok",
                Flags = [],
                QuestionsForReviewer = [],
            });

        var service = new OrderRecommendationService(repo.Object, advisor.Object);

        var result = await service.GetRecommendationAsync(1);

        result.Verdict.Should().Be(AdvisorVerdict.LikelyApprove);
        advisor.Verify(a => a.GetRecommendationAsync(
            order, It.IsAny<RequesterHistory>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRecommendation_WhenOrderMissing_ThrowsNotFound()
    {
        var repo = new Mock<IPurchaseOrderRepository>();
        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PurchaseOrder?)null);

        var advisor = new Mock<IApprovalAdvisor>();
        var service = new OrderRecommendationService(repo.Object, advisor.Object);

        var act = () => service.GetRecommendationAsync(99);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
