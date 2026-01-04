using Watari.Libretro.Bindings;

namespace Watari.Libretro;

public static class PixelConverter
{
    public static unsafe byte[] ConvertFrame(IntPtr data, uint w, uint h, nuint pitch, retro_pixel_format format)
    {
        var pixelBytes = new byte[w * h * 4];
        fixed (byte* dstPtr = pixelBytes)
        {
            if (format == retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888)
            {
                ConvertXRGB8888ToRGBA(data, dstPtr, (int)w, (int)h, (int)pitch);
            }
            else if (format == retro_pixel_format.RETRO_PIXEL_FORMAT_RGB565)
            {
                ConvertRGB565ToRGBA(data, dstPtr, (int)w, (int)h, (int)pitch);
            }
            else if (format == retro_pixel_format.RETRO_PIXEL_FORMAT_0RGB1555)
            {
                Convert0RGB1555ToRGBA(data, dstPtr, (int)w, (int)h, (int)pitch);
            }
            else
            {
                CopyRGBA(data, dstPtr, (int)w, (int)h, (int)pitch);
            }
        }
        return pixelBytes;
    }

    private static unsafe void ConvertXRGB8888ToRGBA(IntPtr src, byte* dst, int w, int h, int pitch)
    {
        for (int y = 0; y < h; y++)
        {
            uint* srcRow = (uint*)(src + y * pitch);
            uint* dstRow = (uint*)(dst + y * w * 4);
            for (int x = 0; x < w; x++)
            {
                uint bgra = srcRow[x];
                uint r = (bgra >> 16) & 0xFF;
                uint g = (bgra >> 8) & 0xFF;
                uint b = bgra & 0xFF;
                uint a = 0xFF;
                dstRow[x] = (r) | (g << 8) | (b << 16) | (a << 24);
            }
        }
    }

    private static unsafe void ConvertRGB565ToRGBA(IntPtr src, byte* dst, int w, int h, int pitch)
    {
        for (int y = 0; y < h; y++)
        {
            ushort* srcRow = (ushort*)(src + y * pitch);
            uint* dstRow = (uint*)(dst + y * w * 4);
            for (int x = 0; x < w; x++)
            {
                ushort rgb565 = srcRow[x];
                uint r = (uint)((rgb565 >> 11) & 0x1F) * 255 / 31;
                uint g = (uint)((rgb565 >> 5) & 0x3F) * 255 / 63;
                uint b = (uint)(rgb565 & 0x1F) * 255 / 31;
                uint a = 0xFF;
                dstRow[x] = (r) | (g << 8) | (b << 16) | (a << 24);
            }
        }
    }

    private static unsafe void Convert0RGB1555ToRGBA(IntPtr src, byte* dst, int w, int h, int pitch)
    {
        for (int y = 0; y < h; y++)
        {
            ushort* srcRow = (ushort*)(src + y * pitch);
            uint* dstRow = (uint*)(dst + y * w * 4);
            for (int x = 0; x < w; x++)
            {
                ushort rgb1555 = srcRow[x];
                uint r = (uint)((rgb1555 >> 10) & 0x1F) * 255 / 31;
                uint g = (uint)((rgb1555 >> 5) & 0x1F) * 255 / 31;
                uint b = (uint)(rgb1555 & 0x1F) * 255 / 31;
                uint a = 0xFF;
                dstRow[x] = (r) | (g << 8) | (b << 16) | (a << 24);
            }
        }
    }

    private static unsafe void CopyRGBA(IntPtr src, byte* dst, int w, int h, int pitch)
    {
        for (int y = 0; y < h; y++)
        {
            Buffer.MemoryCopy((void*)(src + y * pitch), dst + y * w * 4, w * 4, w * 4);
        }
    }
}