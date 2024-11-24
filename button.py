import sys
# interupts = [15, 9]

# read interrupts from args
interupts = []
for i in range(1, len(sys.argv)):
    interupts.append(int(sys.argv[i]))


print("x=20-(i % 20), the natural result")
print("y=abs(x-sum) % 20, the new result not taking into account presses")
print("z=20-<interupt>, how many numbers counted down")
print("sum is the total numbers counted down")
print("x' is the final final number, after all the interrupt(s)")
print()

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
            raise Exception("Interrupt value (" + str(interrupt) + ") is less than the number it would have stopped at (" + str(20-y) + ")")
        z = 20-interupts[i-1]

        print("z=" + str(z), end="\t")
        sum += z
        print("sum=" + str(sum), end="\t")
        print("x'=", z)
    else:
        print("x'=",20-abs(x-sum)%20)