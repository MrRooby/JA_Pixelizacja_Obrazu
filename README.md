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

![image](https://github.com/user-attachments/assets/b4485e1f-418a-4119-a8d5-05d4560128f7)


## Processed Images Example

### Original 4k Image
![pexels-francesco-ungaro-1525041(1)](https://github.com/user-attachments/assets/4a4229d1-4a71-46ea-b15e-1e0dffc928c0)

### 64x pixelization
![64x](https://github.com/user-attachments/assets/29d28cc2-9a1e-47a5-9fc9-7d3ba0761527)

### 256x pixelization
![256x](https://github.com/user-attachments/assets/b5cd4415-924f-41d5-97ce-0791e689678a)

### 1024x pixelization
![1024x](https://github.com/user-attachments/assets/1458668c-559d-47dd-b8b2-f3573dc53594)
