package main;

import io.netty.handler.ssl.SslContext;
import io.netty.handler.ssl.SslContextBuilder;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import javax.crypto.BadPaddingException;
import javax.crypto.Cipher;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.nio.file.StandardCopyOption;
import java.security.InvalidKeyException;
import java.security.KeyFactory;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.NoSuchAlgorithmException;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.security.spec.InvalidKeySpecException;
import java.security.spec.PKCS8EncodedKeySpec;
import java.security.spec.X509EncodedKeySpec;

final public class SSLHandler {
    private static final String PRIVATE_KEY = "TestSync.key";
    private static final String PUBLIC_KEY = "TestSync.crt";
    private static final String MATCHMAKER_PRIVATE_KEY = "Matchmaker.key";
    private static final String MATCHMAKER_PUBLIC_KEY = "Matchmaker.pub";
    private static File publicKey = new File("PUBLIC_KEY");
    private static File privateKey = new File("PRIVATE_KEY");
    private static PrivateKey matchmakerPrivateKey = null;
    private static PublicKey matchmakerPublicKey = null;

    private static final Logger logger = LogManager.getLogger(SSLHandler.class);

    static public SslContext getContext() throws Exception {
        SslContextBuilder contextBuilder = SslContextBuilder.forServer(publicKey, privateKey);
        return contextBuilder.build();
    }

    static public boolean initalizeContext() {
        logger.info("Initializing SSL context...");

        File matchmakerPublicFile = new File(MATCHMAKER_PUBLIC_KEY);
        File matchmakerPrivateFile = new File(MATCHMAKER_PRIVATE_KEY);

        try {
            Files.copy(ClassLoader.getSystemResourceAsStream(PUBLIC_KEY), publicKey.toPath(), StandardCopyOption.REPLACE_EXISTING);
            Files.copy(ClassLoader.getSystemResourceAsStream(PRIVATE_KEY), privateKey.toPath(), StandardCopyOption.REPLACE_EXISTING);
            Files.copy(ClassLoader.getSystemResourceAsStream(MATCHMAKER_PUBLIC_KEY), matchmakerPublicFile.toPath(), StandardCopyOption.REPLACE_EXISTING);
            Files.copy(ClassLoader.getSystemResourceAsStream(MATCHMAKER_PRIVATE_KEY), matchmakerPrivateFile.toPath(), StandardCopyOption.REPLACE_EXISTING);
        } catch (IOException e) {
            e.printStackTrace();
        }

        if (!publicKey.exists() || publicKey.isDirectory()) {
            logger.error("Unable to load public key file. Expected to find file: " + PUBLIC_KEY);
            return false;
        }

        if (!privateKey.exists() || privateKey.isDirectory()) {
            logger.error("Unable to load private key file. Expected to find file: " + PRIVATE_KEY);
            return false;
        }
        
        if (!matchmakerPublicFile.exists() || matchmakerPublicFile.isDirectory()) {
            logger.error("Unable to load public key file. Expected to find file: " + MATCHMAKER_PUBLIC_KEY);
            return false;
        }

        try {
            KeyFactory RSAKeyFactory = KeyFactory.getInstance("RSA");

            byte[] matchmakerPublicKeyBytes = Files.readAllBytes(Paths.get(MATCHMAKER_PUBLIC_KEY));
            byte[] matchmakerPrivateKeyBytes = Files.readAllBytes(Paths.get(MATCHMAKER_PRIVATE_KEY));

            X509EncodedKeySpec publicKeySpec = new X509EncodedKeySpec(matchmakerPublicKeyBytes);
            PKCS8EncodedKeySpec privateKeySpec = new PKCS8EncodedKeySpec(matchmakerPrivateKeyBytes);

            matchmakerPublicKey = RSAKeyFactory.generatePublic(publicKeySpec);
            matchmakerPrivateKey = RSAKeyFactory.generatePrivate(privateKeySpec);
        } catch (NoSuchAlgorithmException | IOException | InvalidKeySpecException e) {
            e.printStackTrace();
        }

        return true;
    }

    public static byte[] encrypt(String data) throws BadPaddingException, IllegalBlockSizeException, InvalidKeyException, NoSuchPaddingException, NoSuchAlgorithmException, UnsupportedEncodingException {
        if (matchmakerPublicKey == null) {
            throw new InvalidKeyException("Public key is null, initialize the SSL context first!");
        }
        Cipher cipher = Cipher.getInstance("RSA");
        cipher.init(Cipher.ENCRYPT_MODE, matchmakerPublicKey);
        return cipher.doFinal(data.getBytes("utf-8"));
    }

    public static String decrypt(byte[] data) throws NoSuchPaddingException, NoSuchAlgorithmException, InvalidKeyException, BadPaddingException, IllegalBlockSizeException {
        if (matchmakerPrivateKey == null) {
            throw new InvalidKeyException("Private key is null, initialize the SSL context first!");
        }
        Cipher cipher = Cipher.getInstance("RSA");
        cipher.init(Cipher.DECRYPT_MODE, matchmakerPrivateKey);
        return new String(cipher.doFinal(data));
    }

    // Utility function used one time to generate matchmaker key pair
    private static void generateKeyPair() throws NoSuchAlgorithmException {
        KeyPairGenerator generator = KeyPairGenerator.getInstance("RSA");
        generator.initialize(4096);
        KeyPair pair = generator.generateKeyPair();

        FileOutputStream privateKeyWriter = null;
        FileOutputStream publicKeyWriter = null;
        try {
            privateKeyWriter = new FileOutputStream(MATCHMAKER_PRIVATE_KEY);
            privateKeyWriter.write(pair.getPrivate().getEncoded());

            publicKeyWriter = new FileOutputStream(MATCHMAKER_PUBLIC_KEY);
            publicKeyWriter.write(pair.getPublic().getEncoded());
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            try {
                if (privateKeyWriter != null) {
                    privateKeyWriter.close();
                }
                if (publicKeyWriter != null) {
                    publicKeyWriter.close();
                }
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

}
