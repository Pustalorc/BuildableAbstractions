using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Pustalorc.Libraries.BuildableAbstractions.Extensions;

/// <summary>
///     A class with extensions that the plugin utilizes.
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    ///     Checks if any element from <paramref name="args" /> is equal to the <see cref="string" />
    ///     <paramref name="include" />.
    /// </summary>
    /// <param name="args">The <see cref="IEnumerable{String}" /> that should be searched.</param>
    /// <param name="include">The <see cref="string" /> that we should find.</param>
    /// <param name="index">
    ///     If <paramref name="include" /> is found in <paramref name="args" />, this will be a number greater than -1 but
    ///     smaller than args.Count
    ///     <br />
    ///     If <paramref name="include" /> isn't found in <paramref name="args" />, this will be -1.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="index" /> is > -1.
    ///     <br />
    ///     <see langword="false" /> if <paramref name="index" /> is == -1.
    /// </returns>
    public static bool CheckArgsIncludeString(this IEnumerable<string> args, string include, out int index)
    {
        index = args.ToList().FindIndex(k => k.Equals(include, StringComparison.OrdinalIgnoreCase));
        return index > -1;
    }

    /// <summary>
    ///     Gets all the of <see cref="ItemAsset" />s that an element of <paramref name="args" />.
    /// </summary>
    /// <param name="args">The <see cref="IEnumerable{String}" /> that should be searched.</param>
    /// <param name="index">
    ///     If any element of <paramref name="args" /> can be an <see cref="ItemAsset" />(s), this will be a number greater
    ///     than -1 but smaller than args.Count
    ///     <br />
    ///     If no elements of <paramref name="args" /> can be an <see cref="ItemAsset" />(s), this will be -1.
    /// </param>
    /// <returns>
    ///     An empty <see cref="List{ItemAsset}" /> if no element in <paramref name="args" /> can be an
    ///     <see cref="ItemAsset" />.
    ///     <br />
    ///     A <see cref="List{ItemAsset}" /> with all the <see cref="ItemAsset" />s from one of the entries in
    ///     <paramref name="args" />.
    /// </returns>
    public static List<ItemAsset> GetMultipleItemAssets(this IEnumerable<string> args, out int index)
    {
        var argsL = args.ToList();
        var assets = Assets.find(EAssetType.ITEM).Cast<ItemAsset>()
            .Where(k => k?.itemName != null && k.name != null).OrderBy(k => k.itemName.Length).ToList();

        for (index = 0; index < argsL.Count; index++)
        {
            var itemAssets = assets.Where(k =>
                argsL[0].Equals(k.id.ToString(), StringComparison.OrdinalIgnoreCase) ||
                argsL[0].Split(' ').All(l => k.itemName.ToLower().Contains(l)) ||
                argsL[0].Split(' ').All(l => k.name.ToLower().Contains(l))).ToList();

            if (itemAssets.Count <= 0)
                continue;

            return itemAssets;
        }

        index = -1;
        return new List<ItemAsset>();
    }

    /// <summary>
    ///     Gets a <see cref="float" /> from an element in <paramref name="args" />.
    /// </summary>
    /// <param name="args">The <see cref="IEnumerable{String}" /> that should be searched.</param>
    /// <param name="index">
    ///     If any element of <paramref name="args" /> is a valid <see cref="float" />, this will be a number greater than -1
    ///     but smaller than args.Count
    ///     <br />
    ///     If no elements of <paramref name="args" /> is a valid <see cref="float" />, this will be -1.
    /// </param>
    /// <returns>
    ///     A <see cref="float" /> from one of the entries in <paramref name="args" />.
    /// </returns>
    public static float GetFloat(this IEnumerable<string> args, out int index)
    {
        var output = float.NegativeInfinity;
        index = args.ToList().FindIndex(k => float.TryParse(k, out output));
        return output;
    }

    /// <summary>
    ///     Gets a <see cref="float" /> from an element in <paramref name="args" />.
    /// </summary>
    /// <param name="args">The <see cref="IEnumerable{String}" /> that should be searched.</param>
    /// <param name="index">
    ///     If any element of <paramref name="args" /> is a valid <see cref="IRocketPlayer" />, this will be a number greater
    ///     than -1 but smaller than args.Count
    ///     <br />
    ///     If no elements of <paramref name="args" /> is a valid <see cref="IRocketPlayer" />, this will be -1.
    /// </param>
    /// <returns>
    ///     <see langword="null" /> if none of the entries in <paramref name="args" /> can be an <see cref="IRocketPlayer" />.
    ///     <br />
    ///     A <see cref="IRocketPlayer" /> from one of the entries in <paramref name="args" />.
    /// </returns>
    public static IRocketPlayer? GetIRocketPlayer(this IEnumerable<string> args, out int index)
    {
        IRocketPlayer? output = null;
        index = args.ToList().FindIndex(k =>
        {
            output = UnturnedPlayer.FromName(k);
            if (output == null && ulong.TryParse(k, out var id) && id > 76561197960265728)
                output = new RocketPlayer(id.ToString());

            return output != null;
        });
        return output;
    }
}