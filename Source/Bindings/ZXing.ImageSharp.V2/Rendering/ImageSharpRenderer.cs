/*
 * Copyright 2017 ZXing.Net authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using ZXing.Common;

namespace ZXing.ImageSharp.Rendering
{
    /// <summary>
    /// IBarcodeRenderer implementation which creates an ImageSharp Image object from the barcode BitMatrix
    /// </summary>
    public class ImageSharpRenderer<TPixel> : ZXing.Rendering.IBarcodeRenderer<Image<TPixel>> where TPixel : unmanaged, IPixel<TPixel>
    {
        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        /// <value>The foreground color.</value>
        public Color Foreground { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        /// <value>The background color.</value>
        public Color Background { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSharpRenderer{TPixel}"/> class.
        /// </summary>
        public ImageSharpRenderer()
        {
            Foreground = Color.Black;
            Background = Color.White;
        }

        /// <summary>
        /// renders the image
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="format"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Image<TPixel> Render(BitMatrix matrix, BarcodeFormat format, string content)
        {
            return Render(matrix, format, content, new EncodingOptions());
        }

        /// <summary>
        /// renders the image
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="format"></param>
        /// <param name="content"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Image<TPixel> Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
        {
            var width = matrix.Width;
            var height = matrix.Height;
            var foreColor = Foreground;
            var backColor = Background;

            var pixelsize = 1;

            if (options != null)
            {
                if (options.Width > width)
                {
                    width = options.Width;
                }
                if (options.Height > height)
                {
                    height = options.Height;
                }
                // calculating the scaling factor
                pixelsize = width / matrix.Width;
                if (pixelsize > height / matrix.Height)
                {
                    pixelsize = height / matrix.Height;
                }
            }

            var result = new Image<TPixel>(width, height);
            for (int y = 0; y < matrix.Height; y++)
            {
                for (var pixelsizeHeight = 0; pixelsizeHeight < pixelsize; pixelsizeHeight++)
                {
                    var rowOffset = pixelsize * y + pixelsizeHeight;

                    for (var x = 0; x < matrix.Width; x++)
                    {
                        var color = matrix[x, y] ? foreColor : backColor;
                        for (var pixelsizeWidth = 0; pixelsizeWidth < pixelsize; pixelsizeWidth++)
                        {
                            var pixel = new TPixel();
                            pixel.FromRgba32(color);
                            result[pixelsize * x + pixelsizeWidth, rowOffset] = pixel;
                        }
                    }
                    for (var x = pixelsize * matrix.Width; x < width; x++)
                    {
                        var pixel = new TPixel();
                        pixel.FromRgba32(foreColor);
                        result[x, rowOffset] = pixel;
                    }
                }
            }

            return result;
        }
    }
}