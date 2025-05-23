﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Wayfarer.Data;

namespace Wayfarer.API;

public static class WayfarerPresets
{
    /// <summary>
    /// A preset method intended for use as <see cref="NavMeshParameters.IsValidNode"/>. This will return true for solid tiles with enough vertical clearance
    /// in the tiles above them to accommodate the hitbox. This method does not check for horizontal clearance!
    /// </summary>
    /// <param name="tile">The position of the tile being checked.</param>
    /// <param name="hitbox">The hitbox of the navigator.</param>
    public static bool DefaultIsTileValid(Point tile, Rectangle hitbox)
    {
        int heightInTiles = (int)Math.Ceiling(hitbox.Height / 16f);

        Tile standingTile = Main.tile[tile];

        // Eliminate tiles that are either non-existent or cannot be stood on.
        if (!standingTile.HasTile || !Main.tileSolid[standingTile.TileType])
        {
            return false;
        }

        // Check tiles above the standing tile to make sure the navigator's hitbox can fit.
        for (int yOffsetTiles = 1; yOffsetTiles < heightInTiles + 1; yOffsetTiles++)
        {
            int offsetTileY = tile.Y - yOffsetTiles;

            Tile offsetTile = Main.tile[tile.X, offsetTileY];

            // Hitbox area is obstructed.
            if (offsetTile.HasTile && Main.tileSolid[offsetTile.TileType])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// A preset method intended for use as <see cref="NavigatorParameters.JumpFunction"/>. Accounting for gravity, this method returns the minimum starting velocity required to jump a navigator from <paramref name="start"/> to <paramref name="end"/>.
    /// </summary>
    /// <param name="start">The world position of the start of the jump.</param>
    /// <param name="end">The world position of the end of the jump.</param>
    /// <param name="gravityFunction">The gravity function of the navigator. For common entities such as NPCs, this can just be <see cref="NPC.gravity"/></param>.
    /// <returns>The minimum starting velocity required to clear the jump.</returns>
    public static Vector2 DefaultJumpFunction(Vector2 start, Vector2 end, Func<float> gravityFunction)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;

        // Invert for +Y = down.
        dy *= -1;

        float r = MathF.Sqrt(dx * dx + dy * dy);

        float gravity = gravityFunction.Invoke();

        float minimalV0 = MathF.Sqrt(gravity * (dy + r));
        float theta = MathF.Atan2(dy + r, dx);

        float ux = minimalV0 * MathF.Cos(theta);
        float uy = minimalV0 * -MathF.Sin(theta);

        uy -= gravity;

        return new Vector2(ux, uy);
    }
}
