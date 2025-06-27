#nullable enable
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace System.Runtime.CompilerServices {
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false )]
    internal sealed class RequiredMemberAttribute : Attribute { }

    [AttributeUsage( AttributeTargets.All, AllowMultiple = true, Inherited = false )]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute {
        public CompilerFeatureRequiredAttribute(string featureName) { }
    }

    [AttributeUsage( AttributeTargets.Parameter, Inherited = false )]
    internal sealed class CallerArgumentExpressionAttribute : Attribute {
        public CallerArgumentExpressionAttribute(string parameterName) {
            ParameterName = parameterName;
        }

        public string ParameterName { get; }
    }

    public static class Unsafe {
        public static bool IsNullRef<T>(ref T value) where T : class {
            // This is a simplified polyfill. The real implementation is an intrinsic.
            return value == null;
        }
    }
}

namespace System.Diagnostics.CodeAnalysis {
    [AttributeUsage( AttributeTargets.Constructor, AllowMultiple = false, Inherited = false )]
    internal sealed class SetsRequiredMembersAttribute : Attribute { }
}

public static class Validation {
    public static void Check(bool condition, [System.Runtime.CompilerServices.CallerArgumentExpression( "condition" )] string? message = null) {
        if ( !condition ) {
            throw new InvalidOperationException( $"Validation failed for: {message}" );
        }
    }
}

[Serializable] public ref struct RefFieldExample {
    public int m_number;
    public void SetNumber(int value) => m_number = value;
    public int GetNumber() => m_number;
}

public record RecordExample(int Id, string Name);
public readonly record struct RecordStructExample(int Id, string Name);

public class TestRefTypes {
    RecordStructExample m_recordStructExample;

    public Action<string> onInfo;

    public void StartTest() {
        TestRefStruct();
        TestRecords();
        TestValidation();
        TestUnsafeIsNullRef();
    }

    void TestRefStruct() {
        var refStruct = new RefFieldExample { m_number = 5 };
        int initialValue = refStruct.GetNumber();
        var initialValueMessage = $"[Ref Struct] Initial value: {initialValue}";
        onInfo?.Invoke( initialValueMessage );
        Debug.Log( initialValueMessage );

        ModifyRefStruct( ref refStruct );
        int modifiedValue = refStruct.GetNumber();
        var modifiedValueMessage = $"[Ref Struct] Modified value: {modifiedValue}";
        onInfo?.Invoke( modifiedValueMessage );
        Debug.Log( modifiedValueMessage );

        Validation.Check( initialValue != modifiedValue, "Ref struct value should have been modified." );
    }

    void ModifyRefStruct(ref RefFieldExample example) {
        example.SetNumber( 10 );
    }

    void TestRecords() {
        // Record Class
        var record1 = new RecordExample( 1, "Test" );
        var record2 = new RecordExample( 1, "Test" );
        var record3 = record1 with { Name = "NewTest" };
        var recordEqualsMessage = $"[Record Class] record1.Equals(record2): {record1.Equals( record2 )} (Expected: True)";
        onInfo?.Invoke( recordEqualsMessage );
        Debug.Log( recordEqualsMessage );
        var recordReferenceEqualsMessage = $"[Record Class] record1 == record2: {record1 == record2} (Expected: False, different references)";
        onInfo?.Invoke( recordReferenceEqualsMessage );
        Debug.Log( recordReferenceEqualsMessage );
        var record3Message = $"[Record Class] record3: {record3}";
        onInfo?.Invoke( record3Message );
        Debug.Log( record3Message );

        // Record Struct
        m_recordStructExample = new RecordStructExample( 1, "Test" );
        var recordStruct2 = new RecordStructExample( 1, "Test" );
        var recordStruct3 = m_recordStructExample with { Name = "NewTest" };
        var recordStructEqualsMessage = $"[Record Struct] m_recordStructExample.Equals(recordStruct2): {m_recordStructExample.Equals( recordStruct2 )} (Expected: True)";
        onInfo?.Invoke( recordStructEqualsMessage );
        Debug.Log( recordStructEqualsMessage );
        var recordStructReferenceEqualsMessage = $"[Record Struct] m_recordStructExample == recordStruct2: {m_recordStructExample == recordStruct2} (Expected: True)";
        onInfo?.Invoke( recordStructReferenceEqualsMessage );
        Debug.Log( recordStructReferenceEqualsMessage );
        var recordStruct3Message = $"[Record Struct] recordStruct3: {recordStruct3}";
        onInfo?.Invoke( recordStruct3Message );
        Debug.Log( recordStruct3Message );
    }

    void TestValidation() {
        const int x = 1;
        const int y = 2;

        // Test passing condition
        try {
            Validation.Check( x < y );
            const string validationPassedMessage = "[Validation] Passed for 'x < y' as expected.";
            onInfo?.Invoke( validationPassedMessage );
            Debug.Log( validationPassedMessage );
        }
        catch (Exception e) {
            Debug.LogException( e );
            var validationFailedMessage = $"[Validation] Failed unexpectedly for 'x < y': {e.Message}";
            onInfo?.Invoke( validationFailedMessage );
            Debug.Log( validationFailedMessage );
        }

        // Test failing condition
        try {
            Validation.Check( x > y );
        }
        catch (Exception e) {
            Debug.LogException( e );
            var validationCaughtMessage = $"[Validation] Caught expected exception for 'x > y': {e.Message}";
            onInfo?.Invoke( validationCaughtMessage );
            Debug.Log( validationCaughtMessage );
        }
    }

    void TestUnsafeIsNullRef() {
        string? notNullObj = "test";
        string? nullObj = null;

        bool isFirstNull = System.Runtime.CompilerServices.Unsafe.IsNullRef( ref notNullObj );
        var firstNullMessage = $"[Unsafe.IsNullRef] Is 'notNullObj' null? {isFirstNull} (Expected: False)";
        onInfo?.Invoke( firstNullMessage );
        Debug.Log( firstNullMessage );

        bool isSecondNull = System.Runtime.CompilerServices.Unsafe.IsNullRef( ref nullObj );
        var secondNullMessage = $"[Unsafe.IsNullRef] Is 'nullObj' null? {isSecondNull} (Expected: True)";
        onInfo?.Invoke( secondNullMessage );
        Debug.Log( secondNullMessage );
    }
}

public class SpanExample {
    public Action<string> onInfo;

    public void StartTest() {
        TestStackAllocSpan();
        TestMemoryMarshalSpan();
    }

    void TestStackAllocSpan() {
        const int size = 4;
        Span<int> tmp = stackalloc int[ size ];
        for (var i = 0; i < size; i++) {
            tmp[i] = i * 10;
        }

        var spanContentMessage = $"[StackAlloc] Span content: {tmp[0]}, {tmp[1]}, {tmp[2]}, {tmp[3]}";
        onInfo?.Invoke( spanContentMessage );
        Debug.Log( spanContentMessage );
        Validation.Check( tmp[3] == 30, "Last element should be 30" );
        const string validationPassedMessage = "[StackAlloc] Validation passed.";
        onInfo?.Invoke( validationPassedMessage );
        Debug.Log( validationPassedMessage );
    }

    void TestMemoryMarshalSpan() {
        var number = 7;
        var initialNumberMessage = $"[MemoryMarshal] Initial number: {number}";
        onInfo?.Invoke( initialNumberMessage );
        Debug.Log( initialNumberMessage );

        Span<int> one = MemoryMarshal.CreateSpan( ref number, 1 );
        one[0] *= 3;

        var modifiedNumberMessage = $"[MemoryMarshal] Modified number via Span: {number}";
        onInfo?.Invoke( modifiedNumberMessage );
        Debug.Log( modifiedNumberMessage );
        Validation.Check( number == 21, "Number should be modified via span." );
        const string validationPassedMessage = "[MemoryMarshal] Validation passed.";
        onInfo?.Invoke( validationPassedMessage );
        Debug.Log( validationPassedMessage );
    }
}