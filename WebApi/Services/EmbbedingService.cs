using WebApi.Dtos;
using WebApi.Extensions;

namespace WebApi.Services;

public class EmbeddingService
{
    private const float Minimum = 0.0f;
    private const float Maximum = 1.0f;

    public static float Truncate(float value) => value switch
    {
        < Minimum => Minimum,
        > Maximum => Maximum,
        _ => value
    };

    public static float[] Embedding(TransactionRequestDto dto, float[]? array = null)
    {
        var amount = Truncate(dto.Transaction.Amount / Constants.MaxAmount);
        var installments = Truncate(dto.Transaction.Installments / Constants.MaxInstallments);
        var amountVsAvg = Truncate(dto.Transaction.Amount / dto.Customer.AvgAmount / Constants.AmountVsAvgRatio);
        var hourOfDay = dto.Transaction.RequestedAt.Hour / 23f;
        var dayOfWeek = dto.Transaction.RequestedAt.DayOfWeekMonToSun / 6f;
        var minutesSinceLastTx = dto.LastTransaction is null
            ? -1
            : Truncate(
                (float)DateTime.UtcNow.Subtract((DateTime)dto.LastTransaction?.Timestamp!).TotalMinutes / Constants.MaxMinutes);
        var kmFromLastTx = dto.LastTransaction is null
            ? -1
            : Truncate((float)dto.LastTransaction?.KmFromCurrent! / Constants.MaxKm);
        var kmFromHome = Truncate(dto.Terminal.KmFromHome / Constants.MaxKm);
        var txCount24H = Truncate(dto.Customer.TxCount24H / Constants.MaxTxCount24H);
        var isOnline = dto.Terminal.IsOnline ? 1f : 0f;
        var cardPresent = dto.Terminal.CardPresent ? 1f : 0f;
        var unknownMerchant = dto.Customer.KnownMerchants.Contains(dto.Merchant.Id) == false ? 1 : 0;
        var mccRisk = Constants.MccRisk.GetValueOrDefault(dto.Merchant.Mcc, 0.5f);
        var merchantAvgAmount = Truncate(dto.Merchant.AvgAmount / Constants.MaxMerchantAvgAmount);

        if (array != null)
        {
            array[0] = amount;
            array[1] = installments;
            array[2] = amountVsAvg;
            array[3] = hourOfDay;
            array[4] = dayOfWeek;
            array[5] = minutesSinceLastTx;
            array[6] = kmFromLastTx;
            array[7] = kmFromHome;
            array[8] = txCount24H;
            array[9] = isOnline;
            array[10] = cardPresent;
            array[11] = unknownMerchant;
            array[12] = mccRisk;
            array[13] = merchantAvgAmount;
        }
        else
        {
            array = [
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

        return array;
    }
}