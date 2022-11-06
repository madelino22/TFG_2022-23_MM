

#method to sort array
def sortArray(array):
    for i in range(len(array)):
        for j in range(len(array)):
            if array[i] < array[j]:
                aux = array[i]
                array[i] = array[j]
                array[j] = aux
    return array


#method to merge sort array
def mergeSort(array):
    if len(array) > 1:
        mid = len(array) // 2 #quita los decimales
        left = array[:mid]
        right = array[mid:]

        mergeSort(left)
        mergeSort(right)

        i = 0
        j = 0
        k = 0

        while i < len(left) and j < len(right):
            if left[i] < right[j]:
                array[k] = left[i]
                i += 1
            else:
                array[k] = right[j]
                j += 1
            k += 1

        while i < len(left):
            array[k] = left[i]
            i += 1
            k += 1

        while j < len(right):
            array[k] = right[j]
            j += 1
            k += 1

    return array