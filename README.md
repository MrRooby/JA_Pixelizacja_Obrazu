# Image Pixelization

## Overview

This program is an image processing application designed to pixelize images using various libraries and techniques. 

Its main purpose is to showcase performance differences between different libraries.

The application provides a graphical user interface (GUI) for users to select images, choose processing parameters, and view the results.  

The pixelization process can be performed using different libraries:
- **C++**
- **ASM**
- **ASM with SIMD** (Single Instruction, Multiple Data) operations.

## Features

- **Image Selection**: Browse and select images for processing.
- **Pixelization**: Apply pixelization effect to images using different libraries.
- **Multi-threading**: Utilize multiple threads for faster processing.
- **Histogram Display**: View histograms of the original and processed images.
- **Save Processed Images**: Save the processed images to your local storage.

## Technologies Used

- **C#**: The main application is developed in C# using **Windows Forms** for the GUI.
- **C++**: A library for pixelization implemented in C++.
- **ASM**: Assembly language libraries for pixelization, including a version with **SIMD** operations for parallel processing.

## Libraries

### C++ Library
The C++ library provides a function to pixelize images by averaging the pixel values within blocks of a specified size.

### ASM Library
The ASM library includes two versions:
- **Standard ASM**: Uses standard assembly instructions for pixelization.
- **SIMD ASM**: Utilizes SIMD (AVX) instructions for parallel processing to enhance performance.

### PixelizeLibraryDelegate
A delegate in C# that defines the signature for the pixelization functions. It allows the application to call the pixelization functions from the C++ and ASM libraries.

## Usage

1. **Select an Image**: Use the "Browse" button to select an image file.
2. **Choose Parameters**: Set the pixel size, number of threads, and select the processing library.
3. **Process Image**: Click the "Process" button to apply the pixelization effect.
4. **View Results**: The original and processed images, along with their histograms, will be displayed.
5. **Save Image**: Optionally, save the processed image to your local storage.

## Program GUI



## Processed Images Example

### Original 4k Image

### 32x pixelization

### 128x pixelization

### 1024x pixelization