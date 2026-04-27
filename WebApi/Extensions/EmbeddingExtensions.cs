using WebApi.DTOs;

namespace WebApi.Extensions;

public static class EmbeddingExtensions
{
    extension(TransactionRequestDto dto)
    {
        public float[] ToEmbedding()
        {
            var amount = Utils.Truncate(dto.Transaction.Amount / Constants.MaxAmount);
            var installments = Utils.Truncate(dto.Transaction.Installments / Constants.MaxInstallments);
            var amountVsAvg =
                Utils.Truncate(dto.Transaction.Amount / dto.Customer.AvgAmount / Constants.AmountVsAvgRatio);
            var hourOfDay = dto.Transaction.RequestedAt.Hour / 23f;
            var dayOfWeek = dto.Transaction.RequestedAt.DayOfWeekMonToSun / 6f;
            var minutesSinceLastTx = dto.LastTransaction is null
                ? -1
                : Utils.Truncate(
                    (float)DateTime.UtcNow.Subtract((DateTime)dto.LastTransaction?.Timestamp!).TotalMinutes /
                    Constants.MaxMinutes);
            var kmFromLastTx = dto.LastTransaction is null
                ? -1
                : Utils.Truncate((float)dto.LastTransaction?.KmFromCurrent! / Constants.MaxKm);
            var kmFromHome = Utils.Truncate(dto.Terminal.KmFromHome / Constants.MaxKm);
            var txCount24H = Utils.Truncate(dto.Customer.TxCount24H / Constants.MaxTxCount24H);
            var isOnline = dto.Terminal.IsOnline ? 1f : 0f;
            var cardPresent = dto.Terminal.CardPresent ? 1f : 0f;
            var unknownMerchant = dto.Customer.KnownMerchants.Any(x => x == dto.Merchant.Id) == false ? 1 : 0;
            var mccRisk = Constants.MccRisk.GetValueOrDefault(dto.Merchant.Mcc, 0.5f);
            var merchantAvgAmount = Utils.Truncate(dto.Merchant.AvgAmount / Constants.MaxMerchantAvgAmount);

            return
            [
                amount,
                installments,
                amountVsAvg,
                hourOfDay,
                dayOfWeek,
                minutesSinceLastTx,
                kmFromLastTx,
                kmFromHome,
                txCount24H,
                isOnline,
                cardPresent,
                unknownMerchant,
                mccRisk,
                merchantAvgAmount
            ];
        }
    }
}