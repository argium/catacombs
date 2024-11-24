# SGHO EIUII ABD MOO NK GHN, EDNY, DL LLSE
import sys


# print("x=20-(i % 20), the natural result")
# print("y=abs(x-sum) % 20, the new result not taking into account presses")
# print("z=20-<interupt>, how many numbers counted down")
# print("sum is the total numbers counted down")
# print("x' is the final final number, after all the interrupt(s)")
# print()

def decode(input, interupts):
    shifts = []
    sum = 0
    x = 20 - (1 % 20)
    for i in range(1,len(interupts)+2):
        print("i=" + str(i), end="\t")

        x = 20 - (i % 20)
        print("x=" + str(x), end="\t")

        y = abs(x - sum) % 20
        print("y=" + str(y), end="\t")

        if i <= len(interupts):
            interrupt = interupts[i-1]
            if interrupt < 20-y:
                raise ValueError("Interrupt value (" + str(interrupt) + ") is less than the number it would have stopped at (" + str(20-y) + ")")
            x2 = 20-interrupt
            shifts.append(interrupt)

            print("z=" + str(x2), end="\t")
            sum += x2
            print("sum=" + str(sum), end="\t")
            print("x'="+str(x2), end="\t")
        else:
            x2 = 20-abs(x-sum)%20
            print("x'="+str(x2), end="\t")
            shifts.append(x2)
        
        # print shifts inline
        for s in shifts:
            print(s, end=" ")
        print()

    # Print how much each character must be shifted
    print("Shifts:", end=" ")
    for s in shifts:
        print(s, end=" ")
    print()

    def new_pos(c, shift):
        d = ord(c)+shift
        # If the new position overflows past 'Z', wrap around to 'A'
        if d > ord('Z'):
            return chr(d-26)
        return chr(d)

    # Apply the shifts to the input string
    output = [new_pos(c, shifts[i]) for i,c in enumerate(ciphertext)]
    output = "".join(output)
    print("Output:"+output)
    return output



# interupts = [15, 9]

# read interrupts from args
ciphertext = sys.argv[1]
print("ciphertext=" + ciphertext)
interupts = []
for i in range(2, len(sys.argv)):
    interupts.append(int(sys.argv[i]))


if len(ciphertext) != len(interupts)+1:
    raise ValueError("Number of interupts must be one less than the length of the ciphertext")

decode(ciphertext, interupts)