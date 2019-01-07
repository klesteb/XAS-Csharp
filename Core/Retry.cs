using System;
using System.Threading;

namespace XAS.Core {

    /// <summary>
    /// A class to retry an opeation.
    /// </summary>
    /// 
    public static class Retry {

        // taken from http://www.mobzystems.com/code/a-functional-retry-pattern/
        // with modifications

        /// <summary>
        /// Retry the specified action at most retries times, or until it returns true.
        /// </summary>
        /// <param name="retries">The number of times to retry the operation.</param>
        /// <param name="delay">The number of seconds to sleep after a failed invocation of the operation.</param>
        /// <param name="operation">The operation to perform. Should return true.</param>
        /// <returns>true if the action returned true on one of the retries, false if the number of retries was exhausted.</returns>
        /// 
        public static bool UntilTrue(Int32 retries, Int32 delay, Func<Boolean> operation) {

            for (var retry = 0; retry < retries; retry++) {

                if (operation()) {

                    return true;

                }

                Thread.Sleep(delay * 1000);

            }

            return false;

        }

        /// <summary>
        /// Retry the specified action at most retries times, or until it returns true.
        /// </summary>
        /// <param name="retries">The number of times to retry the operation.</param>
        /// <param name="delay">The number of seconds to sleep after a failed invocation of the operation.</param>
        /// <param name="token">A CancellationToken object.</param>
        /// <param name="operation">The operation to perform. Should return true.</param>
        /// <returns>true if the action returned true on one of the retries, false if the number of retries was exhausted or a cancellation event happened.</returns>
        /// 
        public static bool UntilTrue(Int32 retries, Int32 delay, CancellationToken token, Func<Boolean> operation) {

            for (var retry = 0; retry < retries; retry++) {

                if (operation()) {

                    return true;

                }

                if (Utils.Sleep(delay, token)) {

                    return false;

                }

            }

            return false;

        }

        /// <summary>
        /// Retry the specified action at most retries times, or until it returns true. Where retires is
        /// an array of increasing wait times. 
        /// </summary>
        /// <param name="retries">The number of times to retry the operation.</param>
        /// <param name="operation">The operation to perform. Should return true.</param>
        /// <returns>true if the action returned true on one of the retries, false if the number of retries was exhausted.</returns>
        /// 
        public static bool UntilTrue(Int32[] retries, Func<Boolean> operation) {

            for (int x = 0; x < retries.Length; x++) {

                if (operation()) {
                
                    return true;

                }

                Thread.Sleep(retries[x] * 1000);

            }

            return false;

        }


        /// <summary>
        /// Retry the specified action at most retries times, or until it returns true. Where retires is
        /// an array of increasing wait times. 
        /// </summary>
        /// <param name="retries">The number of times to retry the operation.</param>
        /// <param name="token">A CancellationToken object.</param>
        /// <param name="operation">The operation to perform. Should return true.</param>
        /// <returns>true if the action returned true on one of the retries, false if the number of retries was exhausted or a cancellation event happened.</returns>
        /// 
        public static bool UntilTrue(Int32[] retries, CancellationToken token, Func<Boolean> operation) {

            for (int x = 0; x < retries.Length; x++) {

                if (operation()) {

                    return true;

                }

                if (Utils.Sleep(retries[x], token)) {

                    return false;

                }

            }

            return false;

        }

        /// <summary>
        /// Retry the specified operation the specified number of times, or until there are no more retries, or it succeeded
        /// without an exception.
        /// </summary>
        /// <typeparam name="T">The return type of the exception.</typeparam>
        /// <param name="retries">The number of times to retry the operation.</param>
        /// <param name="delay">The number of seconds to sleep after a failed invocation of the operation.</param>
        /// <param name="operation">the operation to perform.</param>
        /// <param name="exceptionType">if not null, ignore any exceptions of this type and subtypes.</param>
        /// <param name="allowDerivedExceptions">If true, exceptions deriving from the specified exception type are ignored as well. Defaults to False.</param>
        /// <returns>When one of the retries succeeds, return the value the operation returned. If not, an exception is thrown.</returns>
        /// 
        public static T WhileException<T>(Int32 retries, Int32 delay, Func<T> operation, Type exceptionType = null, 
                                          Boolean allowDerivedExceptions = false ) {

            // Do all but one retries in the loop

            for (var retry = 1; retry < retries; retry++) {

                try {

                    // Try the operation. If it succeeds, return its result

                    return operation();

                } catch (Exception ex) {

                    // Oops - it did NOT succeed!

                    if (exceptionType == null ||
                        ex.GetType().Equals(exceptionType) ||
                        (allowDerivedExceptions && ex.GetType().IsSubclassOf(exceptionType))) {

                        // Ignore exceptions when exceptionType is not specified OR
                        // the exception thrown was of the specified exception type OR
                        // the exception thrown is derived from the specified exception type and we allow that

                        Thread.Sleep(delay * 1000);

                    } else {

                        // We have an unexpected exception! Re-throw it:

                        throw;

                    }

                }

            }

            // Try the operation one last time. This may or may not succeed.
            // Exceptions pass unchanged. If this is an expected exception we need to know about it because
            // we're out of retries. If it's unexpected, throwing is the right thing to do anyway

            return operation();

        }

        /// <summary>
        /// Retry the specified operation the specified number of times, or until there are no more retries, or it succeeded
        /// without an exception.
        /// </summary>
        /// <typeparam name="T">The return type of the exception.</typeparam>
        /// <param name="retries">The number of times to retry the operation.</param>
        /// <param name="delay">The number of seconds to sleep after a failed invocation of the operation.</param>
        /// <param name="token">A CancellationToken object.</param>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="exceptionType">if not null, ignore any exceptions of this type and subtypes.</param>
        /// <param name="allowDerivedExceptions">If true, exceptions deriving from the specified exception type are ignored as well. Defaults to False</param>
        /// <returns>When one of the retries succeeds, return the value the operation returned. If not, an exception is thrown.</returns>
        /// 
        public static T WhileException<T>(Int32 retries, Int32 delay, CancellationToken token, Func<T> operation, Type exceptionType = null,
                                          Boolean allowDerivedExceptions = false) {

            // Do all but one retries in the loop

            for (var retry = 1; retry < retries; retry++) {

                try {

                    // Try the operation. If it succeeds, return its result

                    return operation();

                } catch (Exception ex) {

                    // Oops - it did NOT succeed!

                    if (exceptionType == null ||
                        ex.GetType().Equals(exceptionType) ||
                        (allowDerivedExceptions && ex.GetType().IsSubclassOf(exceptionType))) {

                        // Ignore exceptions when exceptionType is not specified OR
                        // the exception thrown was of the specified exception type OR
                        // the exception thrown is derived from the specified exception type and we allow that

                        if (Utils.Sleep(delay, token)) {

                            throw new OperationCanceledException("A cancellation request was received");

                        }

                    } else {

                        // We have an unexpected exception! Re-throw it:

                        throw;

                    }

                }

            }

            // Try the operation one last time. This may or may not succeed.
            // Exceptions pass unchanged. If this is an expected exception we need to know about it because
            // we're out of retries. If it's unexpected, throwing is the right thing to do anyway

            return operation();

        }

    }

}
