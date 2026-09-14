namespace SmartX.Api.Models;

public readonly record struct SmartMeterValue(double Value)
{
    public static SmartMeterValue operator +(
        SmartMeterValue first,
        SmartMeterValue second)
    {
        return new SmartMeterValue(first.Value + second.Value);
    }

    public static SmartMeterValue operator -(
        SmartMeterValue first,
        SmartMeterValue second)
    {
        return new SmartMeterValue(first.Value - second.Value);
    }

    public static bool operator >(
        SmartMeterValue first,
        SmartMeterValue second)
    {
        return first.Value > second.Value;
    }

    public static bool operator <(
        SmartMeterValue first,
        SmartMeterValue second)
    {
        return first.Value < second.Value;
    }

    public static bool operator >=(
        SmartMeterValue first,
        SmartMeterValue second)
    {
        return first.Value >= second.Value;
    }

    public static bool operator <=(
        SmartMeterValue first,
        SmartMeterValue second)
    {
        return first.Value <= second.Value;
    }
}