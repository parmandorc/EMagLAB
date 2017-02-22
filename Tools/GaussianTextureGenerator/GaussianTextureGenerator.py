'''
This is a tool that has been developed for the generation of radial gradients,
in order to be able to have gaussian gradient textures.
'''
import sys
import cv2
import numpy as np

def gaussian(xy, mu, sig):
    return np.exp(-np.sum(np.power(xy - mu, 2.0) / (2.0 * np.power(sig, 2.0)), axis = 2))

if __name__ == "__main__":

    # Handle the input parameters

    if len(sys.argv) < 2 or sys.argv[1] == "-h" or sys.argv[1] == "--help":
        print "GaussianTextureGenerator"
        print "Generate a square texture image, with a black to white radial gaussian gradient."
        print "Use: python GaussianTextureGenerator.py <output_path> <size> [<rescale_factor>]"
        print "\t output_path: the path where the output image will be saved."
        print "\t sizeh: the size in pixels of the square image."
        print "\t rescale_factor: the rescaling factor to apply to the gaussian (default: 1.0). " + \
            "Higher values will effectively make the black center bigger in size. " + \
            "Lower values will make the center grayer instead of black."
        sys.exit(0)

    path = sys.argv[1]

    if len(sys.argv) < 3:
        print "Error: must specify the size of the image in pixels."
        sys.exit(1)

    try:
        size = int(sys.argv[2])
    except ValueError:
        size = -1

    if size < 2:
        print "Error: '" + str(sys.argv[2]) + "' is not a valid image size. Minimum size: 2"
        print sys.exit(1)

    rescaleFactor = 1.0
    if len(sys.argv) > 3:
        try:
            rescaleFactor = float(sys.argv[3])
        except ValueError:
            rescaleFactor = -1

        if rescaleFactor <= 0:
            print "Error: '" + str(sys.argv[3]) + "' is not a valid rescaling factor."
            print sys.exit(1)


    # Texture generation
    coords = np.dstack(np.meshgrid(np.arange(size), np.arange(size), indexing = "ij"))
    img = gaussian(coords, (size - 1) * 0.5, (size - 1) * 0.166666666667)

    # Soften outter radius
    min = img[0, int((size - 1) * 0.5)]
    img = (img - min) / (1 - min)

    # Rescale and invert
    img = 1 - rescaleFactor * img

    # Save the texture
    cv2.imwrite(path, img * 255)

