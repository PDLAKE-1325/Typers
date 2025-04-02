
class Stack {
private:
    int data[100];  // 내부 데이터 감춤
    int top;

public:
    Stack() { top = -1; }  
    void push(int value) { data[++top] = value; }
    int pop() { return data[top--]; }
    bool isEmpty() { return top == -1; }
};

// #include <stdio.h>
// int stack[100];
// int top = -1;
// void push(int value){
// 	stack[++top] = value;
// }
// int pop(){
// 	return top==-1?-1:stack[top--];
// }
// int main(){
//     printf("%d",pop());
// }